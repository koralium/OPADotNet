using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryContextPolicies : SyncContextIterationPolicies
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly DiscoveryHandler _discoveryHandler;

        internal DiscoveryContextPolicies(OpaClientEmbedded opaClientEmbedded, DiscoveryHandler discoveryHandler) : base(opaClientEmbedded, null, null)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _discoveryHandler = discoveryHandler;
        }

        public override SyncContextIterationData Next()
        {
            List<SyncPolicy> syncPolicies = new List<SyncPolicy>();

            foreach(var addedPolicy in AddedPolicies)
            {
                syncPolicies.Add(new SyncPolicy()
                {
                    DataSets = new HashSet<string>(),
                    PolicyName = addedPolicy.Id,
                    Raw = addedPolicy.Raw
                });
            }

            return new DiscoveryContextData(_opaClientEmbedded, syncPolicies, _discoveryHandler);
        }
    }
}
