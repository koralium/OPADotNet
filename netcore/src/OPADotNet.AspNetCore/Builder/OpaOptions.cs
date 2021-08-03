using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class OpaOptions
    {
        public Uri OpaServer { get; }

        public bool UseEmbedded { get; }

        public SyncOptions SyncOptions { get; }

        public DiscoveryOptions DiscoveryOptions { get; }

        public OpaOptions(
            Uri opaServer,
            bool useEmbedded,
            SyncOptions syncOptions,
            DiscoveryOptions discoveryOptions)
        {
            OpaServer = opaServer;
            UseEmbedded = useEmbedded;
            SyncOptions = syncOptions;
            DiscoveryOptions = discoveryOptions;
        }
    }
}
