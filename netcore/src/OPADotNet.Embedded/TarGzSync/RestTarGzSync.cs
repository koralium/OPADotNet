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

        public RestTarGzSync(RestTarGzOptions options)
        {
            _options = options;
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
                    //TODO: Add logging etc
                }
            }
        }

        public override async Task GetTarGzStream(Func<Stream, Task> addStream)
        {
            HttpClient httpClient = new HttpClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, _options.Url);

            foreach(var header in _options.Headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            if (_options.CredentialMethod != null)
            {
                await _options.CredentialMethod.Apply(requestMessage);
            }

            var response = await httpClient.SendAsync(requestMessage);

            using var stream = await response.Content.ReadAsStreamAsync();
            await addStream(stream);
        }
    }
}
