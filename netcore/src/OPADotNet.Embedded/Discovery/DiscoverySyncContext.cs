using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoverySyncContext : SyncContext
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly DiscoveryHandler _discoveryHandler;

        internal DiscoverySyncContext(OpaClientEmbedded opaClientEmbedded, DiscoveryHandler discoveryHandler) : base(new List<SyncPolicyDescriptor>(), opaClientEmbedded)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _discoveryHandler = discoveryHandler;
        }

        public override SyncContextIterationPolicies NewIteration()
        {
            return new DiscoveryContextPolicies(_opaClientEmbedded, _discoveryHandler);
        }
    }
}
