using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    /// <summary>
    /// Service that helps with synchronisation
    /// </summary>
    public class SyncContext
    {
        private readonly List<SyncPolicyDescriptor> _modules;
        private readonly OpaClientEmbedded _opaClientEmbedded;

        private readonly Dictionary<string, SyncPolicy> _existingSyncPolicies = new Dictionary<string, SyncPolicy>();

        internal SyncContext(List<SyncPolicyDescriptor> modules, OpaClientEmbedded opaClientEmbedded)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _modules = modules;
        }

        public SyncContextIterationPolicies NewIteration()
        {
            return new SyncContextIterationPolicies(_opaClientEmbedded, _modules, _existingSyncPolicies);
        }
    }
}
