using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    class LocalTarGzSync : TarGzSync
    {
        private readonly string _filePath;

        public LocalTarGzSync(string filePath)
        {
            _filePath = filePath;
        }

        public override Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override async Task GetTarGzStream(Func<Stream, Task> addStream)
        {
            using var stream = File.OpenRead(_filePath);
            await addStream(stream);
        }
    }
}
