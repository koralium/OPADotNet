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
