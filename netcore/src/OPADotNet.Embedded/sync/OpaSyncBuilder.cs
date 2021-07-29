using OPADotNet.Ast.Models;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public class OpaSyncBuilder
    {
        private RestOpaClient _restClient;
        private List<SyncPolicyDescriptor> _modules = new List<SyncPolicyDescriptor>();
        private OpaClientEmbedded _clientEmbedded;
        private TimeSpan _syncTime = TimeSpan.FromMinutes(5);
        private List<Policy> _locallyManagedModules = new List<Policy>();

        /// <summary>
        /// Add the rest client to use for the sync
        /// </summary>
        /// <param name="restOpaClient"></param>
        /// <returns></returns>
        public OpaSyncBuilder SetRestClient(RestOpaClient restOpaClient)
        {
            _restClient = restOpaClient;
            return this;
        }

        /// <summary>
        /// Add module to sync
        /// </summary>
        /// <param name="module">The module name to sync</param>
        /// <param name="unknown">The unknown data in the module</param>
        /// <returns></returns>
        public OpaSyncBuilder AddModule(string module, string unknown)
        {
            _modules.Add(new SyncPolicyDescriptor()
            {
                PolicyName = module,
                Unknown = unknown
            });
            return this;
        }

        public OpaSyncBuilder AddLocalManagedModule(Policy policy)
        {
            _locallyManagedModules.Add(policy);
            return this;
        }

        /// <summary>
        /// Set the embedded client to sync data to.
        /// </summary>
        /// <param name="opaClientEmbedded"></param>
        /// <returns></returns>
        public OpaSyncBuilder SetEmbeddedClient(OpaClientEmbedded opaClientEmbedded)
        {
            _clientEmbedded = opaClientEmbedded;
            return this;
        }

        public OpaSyncBuilder SyncTime(TimeSpan syncTime)
        {
            _syncTime = syncTime;
            return this;
        }

        public OpaSync Build()
        {
            return new OpaSync(_restClient, _clientEmbedded, _modules, _syncTime, _locallyManagedModules);
        }
    }
}
