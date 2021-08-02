using OPADotNet.Ast.Models;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryContextData : SyncContextIterationData
    {
        internal DiscoveryContextData(OpaClientEmbedded opaClientEmbedded, List<SyncPolicy> policies) 
            : base(opaClientEmbedded, policies, new DataSetNode("data", false)
                {
                    UsedInPolicyMutable = true
                })
        {
        }
    }
}
