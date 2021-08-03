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
