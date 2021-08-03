/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var authPolicyProvider = _serviceProvider.GetService(typeof(IAuthorizationPolicyProvider));

            if (authPolicyProvider == null)
            {
                throw new InvalidOperationException("Cannot add OPA without adding AddAuthorization in services");
            }

            var requirements = RequirementsStore.GetRequirements();

            var embeddedClient = _serviceProvider.GetService(typeof(OpaClientEmbedded)) as OpaClientEmbedded;

            if (_opaOptions.UseEmbedded)
            {
                List<SyncPolicyDescriptor> syncPolicyDescriptors = new List<SyncPolicyDescriptor>();
                foreach (var requirement in requirements)
                {
                    syncPolicyDescriptors.Add(new SyncPolicyDescriptor()
                    {
                        PolicyName = requirement.ModuleName,
                        Unknown = requirement.DataName
                    });
                }

                DiscoveryHandler discoveryHandler = new DiscoveryHandler(
                    syncPolicyDescriptors,
                    _serviceProvider
                    );

                await discoveryHandler.Start();
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
