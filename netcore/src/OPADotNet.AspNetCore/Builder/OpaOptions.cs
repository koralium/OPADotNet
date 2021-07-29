using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class OpaOptions
    {
        public Uri OpaServer { get; }

        public IReadOnlyList<OpaBuilderPolicy> Policies { get; }

        public bool UseEmbedded { get; }

        public TimeSpan SyncTime { get; }

        public OpaOptions(Uri opaServer, IReadOnlyList<OpaBuilderPolicy> policies, bool useEmbedded, TimeSpan syncTime)
        {
            OpaServer = opaServer;
            Policies = policies;
            UseEmbedded = useEmbedded;
            SyncTime = syncTime;
        }
    }
}
