using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore
{
    class FinalizeWorker : IHostedService
    {
        private readonly PreparedPartialStore _preparedPartialStore;
        private readonly OpaOptions _opaOptions;
        private readonly IServiceProvider _serviceProvider;
        public FinalizeWorker(
            PreparedPartialStore preparedPartialStore,
            OpaOptions opaOptions,
            IServiceProvider serviceProvider)
        {
            _preparedPartialStore = preparedPartialStore;
            _opaOptions = opaOptions;
            _serviceProvider = serviceProvider;
        }

        private List<Policy> GetLocalPolicies()
        {
            Dictionary<string, string> localPolicies = new Dictionary<string, string>();
            foreach(var localModule in _opaOptions.Policies)
            {
                localPolicies.Add(localModule.PolicyName, localModule.Module);
            }
            var compiler = new OpaCompiler(localPolicies);
            var astPolicies = compiler.GetPolicies();
            compiler.Dispose();

            List<Policy> output = new List<Policy>();
            foreach (var localModule in _opaOptions.Policies)
            {
                output.Add(new Policy()
                {
                    Ast = astPolicies[localModule.PolicyName],
                    Id = localModule.PolicyName,
                    Raw = localModule.Module
                });
            }
            return output;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var authPolicyProvider = _serviceProvider.GetService(typeof(IAuthorizationPolicyProvider));

            if (authPolicyProvider == null)
            {
                throw new InvalidOperationException("Cannot add OPA without adding AddAuthorization in services");
            }

            var requirements = RequirementsStore.GetRequirements();

            bool syncEnabled = false;

            var embeddedClient = _serviceProvider.GetService(typeof(OpaClientEmbedded)) as OpaClientEmbedded;

            if (_opaOptions.SyncTime != TimeSpan.Zero)
            {
                var restClient = _serviceProvider.GetService(typeof(RestOpaClient)) as RestOpaClient;
                

                if (restClient != null && embeddedClient != null)
                {
                    syncEnabled = true;
                    var localPolicies = GetLocalPolicies();

                    OpaSyncBuilder opaSyncBuilder = new OpaSyncBuilder();

                    foreach (var localPolicy in localPolicies)
                    {
                        opaSyncBuilder.AddLocalManagedModule(localPolicy);
                    }

                    foreach (var requirement in requirements)
                    {
                        opaSyncBuilder.AddModule(requirement.ModuleName, requirement.DataName);
                    }

                    opaSyncBuilder.SetRestClient(restClient);
                    opaSyncBuilder.SetEmbeddedClient(embeddedClient);
                    opaSyncBuilder.SyncTime(_opaOptions.SyncTime);

                    var sync = opaSyncBuilder.Build();
                    await sync.Start();
                }
            }

            if (!syncEnabled && embeddedClient != null)
            {
                //Add all the local policies directly since sync is not enabled
                var txn = embeddedClient.OpaStore.NewTransaction();

                foreach (var policy in _opaOptions.Policies)
                {
                    txn.UpsertPolicy(policy.PolicyName, policy.Module);
                }

                txn.Commit();
            }

            foreach(var requirement in requirements)
            {
                _preparedPartialStore.PreparePartial(requirement);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
