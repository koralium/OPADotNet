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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    /// <summary>
    /// Handles the data step of the sync process
    /// </summary>
    public class SyncContextIterationData : ISyncContextData
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<SyncPolicy> _policies;
        private readonly OpaTransaction _opaTransaction;
        private readonly DataSetNode _tree;

        internal SyncContextIterationData(OpaClientEmbedded opaClientEmbedded, List<SyncPolicy> policies, DataSetNode tree)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _opaTransaction = _opaClientEmbedded.OpaStore.NewTransaction(true);
            _policies = policies;
            _tree = tree;
        }

        public IReadOnlyDictionary<string, DataSetNode> DataSets => _tree.Children;

        public DataSetNode RootDataNode => _tree;

        public void AddData(string path, string content)
        {
            if (!path.StartsWith("/"))
            {
                throw new InvalidOperationException($"Path must start with '/', but got: '{path}'");
            }
            _opaTransaction.WriteJson(path, content);
        }

        /// <summary>
        /// Mark that the sync iteration is done, and the changes will be propagated to the embedded OPA server
        /// </summary>
        public virtual Task Done()
        {
            //Write the policies into the store
            foreach (var usedPolicy in _policies)
            {
                foreach(var kv in usedPolicy.Raw)
                {
                    _opaTransaction.UpsertPolicy(kv.Key, kv.Value);
                }
            }

            //Commit all the data changes and policies
            _opaTransaction.Commit();

            return Task.CompletedTask;
        }
    }
}
