using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoverySyncContext : SyncContext
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;

        internal DiscoverySyncContext(OpaClientEmbedded opaClientEmbedded) : base(new List<SyncPolicyDescriptor>(), opaClientEmbedded)
        {
            _opaClientEmbedded = opaClientEmbedded;
        }

        public override SyncContextIterationPolicies NewIteration()
        {
            return new DiscoveryContextPolicies(_opaClientEmbedded);
        }
    }
}
