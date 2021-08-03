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
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpaBuilderExtensions
    {
        public static ISyncBuilder UseOpaServer(this ISyncBuilder syncBuilder, string opaServerUrl, TimeSpan interval = default)
        {
            return syncBuilder.UseOpaServer(opt =>
            {
                opt.OpaServerUrl = opaServerUrl;

                if (interval != default)
                {
                    opt.Interval = interval;
                }
            });
        }

        public static ISyncBuilder UseOpaServer(this ISyncBuilder syncBuilder, Action<OpaServerSyncOptions> options)
        {
            OpaServerSyncOptions opt = new OpaServerSyncOptions();
            options?.Invoke(opt);

            syncBuilder.Services.AddSingleton(opt);
            syncBuilder.AddSyncService<OpaServerSync>();

            return syncBuilder;
        }

        public static ISyncBuilder UseLocal(this ISyncBuilder syncBuilder, Action<LocalSyncOptions> options)
        {
            LocalSyncOptions localSyncOptions = new LocalSyncOptions();
            options?.Invoke(localSyncOptions);

            syncBuilder.Services.AddSingleton(localSyncOptions);
            syncBuilder.AddSyncService<LocalSync>();
            return syncBuilder;
        }

        public static ISyncBuilder UseLocalTarGz(this ISyncBuilder syncBuilder, string filePath)
        {
            return syncBuilder.AddSyncService(new LocalTarGzSync(filePath));
        }

        public static ISyncBuilder UseHttpTarGz(this ISyncBuilder syncBuilder, Action<RestTarGzOptions> options)
        {
            RestTarGzOptions restTarGzOptions = new RestTarGzOptions();
            options?.Invoke(restTarGzOptions);
            syncBuilder.AddSyncService(new RestTarGzSync(restTarGzOptions));
            return syncBuilder;
        }
    }
}
