using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryOptions
    {
        public SyncServiceHolder DiscoveryService { get; }

        public string Decision { get; }

        public DiscoveryOptions(SyncServiceHolder discoveryService, string decision)
        {
            DiscoveryService = discoveryService;
            Decision = decision;
        }
    }
}
