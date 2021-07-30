using OPADotNet.AspNetCore.Builder;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpaBuilderExtensions
    {
        public static OpaBuilder OpaServerSync(this OpaBuilder opaBuilder, string opaServerUrl, TimeSpan interval = default)
        {
            return opaBuilder.OpaServerSync(opt =>
            {
                opt.OpaServerUrl = opaServerUrl;

                if (interval != default)
                {
                    opt.Interval = interval;
                }
            });
        }

        public static OpaBuilder OpaServerSync(this OpaBuilder opaBuilder, Action<OpaServerSyncOptions> options)
        {
            OpaServerSyncOptions opt = new OpaServerSyncOptions();
            options?.Invoke(opt);

            opaBuilder.Services.AddSingleton(opt);
            opaBuilder.AddSyncService<OpaServerSync>();

            return opaBuilder;
        }

        public static OpaBuilder Local(this OpaBuilder opaBuilder, Action<LocalSyncOptions> options)
        {
            LocalSyncOptions localSyncOptions = new LocalSyncOptions();
            options?.Invoke(localSyncOptions);

            opaBuilder.Services.AddSingleton(localSyncOptions);
            opaBuilder.AddSyncService<LocalSync>();
            return opaBuilder;
        }

        public static OpaBuilder LoadLocalTarGz(this OpaBuilder opaBuilder, string filePath)
        {
            return opaBuilder.AddSyncService(new LocalTarGzSync(filePath));
        }
    }
}
