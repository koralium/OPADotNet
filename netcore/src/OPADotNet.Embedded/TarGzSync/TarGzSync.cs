/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    public abstract class TarGzSync : SyncServiceBase<TarGzSync>
    {
        private const string TmpFileLocation = "tmp/tarSync.tar.gz";

        private FileStream temporaryFileStream;

        private void BeginLoad()
        {
            if (temporaryFileStream != null)
            {
                throw new InvalidOperationException("Cannot begin a new sync with tar.gz when another one is already running.");
            }

            var tmpFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileStream outStream = new FileStream(tmpFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            temporaryFileStream = outStream;
        }

        private void EndLoad()
        {
            if (temporaryFileStream == null)
            {
                throw new InvalidOperationException("Incorrect state, trying to end tar.gz state without beginning it.");
            }
            temporaryFileStream.Dispose();
            temporaryFileStream = null;
        }

        private void ReadData(FileStream tmpStream, ISyncContextData dataStep)
        {
            tmpStream.Seek(0, SeekOrigin.Begin);
            using (GZipInputStream gzipStream = new GZipInputStream(tmpStream))
            {
                using (TarInputStream tarIn = new TarInputStream(gzipStream, Encoding.UTF8))
                {
                    TarEntry tarEntry;
                    while ((tarEntry = tarIn.GetNextEntry()) != null)
                    {
                        if (tarEntry.IsDirectory)
                        {
                            continue;
                        }
                        
                        var fileName = Path.GetFileName(tarEntry.Name);
                        if (fileName == "data.json")
                        {
                            var directoryName = Path.GetDirectoryName(tarEntry.Name).Replace("\\", "/");
                            var dataSetNode = FindDataSetNode(directoryName, dataStep.RootDataNode);

                            //Read data
                            if (dataSetNode != null)
                            {
                                //Read the data file
                                string dataJson = DataJsonReader.FilterJson(tarIn, dataSetNode);
                                var path = $"/{directoryName}";
                                dataStep.AddData(path, dataJson);
                            }
                        }
                    }
                }
            }
        }

        private DataSetNode FindDataSetNode(string path, DataSetNode node)
        {
            var nodeNames = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToList();

            for (int i = 0; i < nodeNames.Count; i++)
            {
                if (node.Children.TryGetValue(nodeNames[i], out var childNode))
                {
                    node = childNode;
                }
                else
                {
                    return null;
                }
            }

            return node;
        }

        private List<(string fileName, string policyText)> ReadPolicies(Stream inputStream, FileStream tmpFileStream)
        { 
            using GZipOutputStream gzoStream = new GZipOutputStream(tmpFileStream);
            gzoStream.IsStreamOwner = false;
            gzoStream.SetLevel(3);
            using TarOutputStream tarOutputStream = new TarOutputStream(gzoStream, Encoding.UTF8);
            tarOutputStream.IsStreamOwner = false;

            List<(string fileName, string policyText)> policies = new List<(string fileName, string policyText)>();

            using GZipInputStream gzipStream = new GZipInputStream(inputStream);
            using TarInputStream tarIn = new TarInputStream(gzipStream, Encoding.UTF8);
            TarEntry tarEntry;
            while ((tarEntry = tarIn.GetNextEntry()) != null)
            {
                if (tarEntry.IsDirectory)
                {
                    tarOutputStream.PutNextEntry(tarEntry);
                    continue;
                }

                var fileName = Path.GetFileName(tarEntry.Name);
                if (fileName == "data.json")
                {
                    tarOutputStream.PutNextEntry(tarEntry);
                    tarIn.CopyEntryContents(tarOutputStream);
                    tarOutputStream.CloseEntry();
                    tarOutputStream.Flush();
                }
                else if (fileName.EndsWith(".rego"))
                {
                    using var streamReader = new StreamReader(tarIn, Encoding.UTF8, true, 4096, true);
                    var policyText = streamReader.ReadToEnd();
                    policies.Add((tarEntry.Name, policyText));
                }
            }

            return policies;
        }

        public abstract Task GetTarGzStream(Func<Stream, Task> addStream);

        public override async Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken)
        {
            BeginLoad();
            TaskCompletionSource<Stream> getStreamTask = new TaskCompletionSource<Stream>();
            TaskCompletionSource<object> doneWithStreamTask = new TaskCompletionSource<object>();


            Task getTarGzStreamTask = null;
            getTarGzStreamTask = GetTarGzStream(stream =>
            {
                getStreamTask.SetResult(stream);
                return doneWithStreamTask.Task;
            });

            //Wait for both tasks first, since an exception might be thrown
            await Task.WhenAny(getStreamTask.Task, getTarGzStreamTask);

            if (getTarGzStreamTask.IsFaulted)
            {
                throw getTarGzStreamTask.Exception;
            }

            //Wait to get the stream
            var stream = await getStreamTask.Task;

            var policies = ReadPolicies(stream, temporaryFileStream);

            //Mark that usage with the input stream is done.
            doneWithStreamTask.SetResult(null);

            //Wait for the stream to be closed.
            await getTarGzStreamTask;

            foreach (var policy in policies)
            {
                var compiledPolicy = syncContextPolicies.CompilePolicy(policy.fileName, policy.policyText);
                syncContextPolicies.AddPolicy(compiledPolicy);
            }
        }

        public override Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken)
        {
            if (syncContextData.DataSets.Count > 0)
            {
                ReadData(temporaryFileStream, syncContextData);
            }
            EndLoad();
            return Task.CompletedTask;
        }
    }
}
