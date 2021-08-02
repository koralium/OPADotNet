using Microsoft.Extensions.Logging;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    internal class RestTarGzSync : TarGzSync
    {
        private readonly RestTarGzOptions _options;
        private readonly ILogger _logger;

        public RestTarGzSync(RestTarGzOptions options, ILogger<RestTarGzSync> logger)
        {
            _options = options;
            _logger = logger;
        }

        public override async Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_options.Interval);
                    await FullLoad(syncContext, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Could not update policies and data from Rest tar.gz service.");
                }
            }
        }

        public override async Task GetTarGzStream(Func<Stream, Task> addStream)
        {
            HttpClient httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync(_options.Url);
            await addStream(stream);
        }
    }
}
