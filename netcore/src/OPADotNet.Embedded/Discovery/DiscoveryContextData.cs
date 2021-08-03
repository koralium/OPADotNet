using OPADotNet.Ast.Models;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryContextData : SyncContextIterationData
    {
        private readonly DiscoveryHandler _discoveryHandler;

        internal DiscoveryContextData(OpaClientEmbedded opaClientEmbedded, List<SyncPolicy> policies, DiscoveryHandler discoveryHandler) 
            : base(opaClientEmbedded, policies, new DataSetNode("data", false)
                {
                    UsedInPolicyMutable = true
                })
        {
            _discoveryHandler = discoveryHandler;
        }

        public override async Task Done()
        {
            await base.Done();

            //Update the discovery handler
            await _discoveryHandler.Update();
        }
    }
}
