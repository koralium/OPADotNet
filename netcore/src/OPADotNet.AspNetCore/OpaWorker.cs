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
using Microsoft.Extensions.Logging;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore
{
    class OpaWorker : IHostedService
    {
        private readonly PreparedPartialStore _preparedPartialStore;
        private readonly OpaOptions _opaOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public OpaWorker(
            PreparedPartialStore preparedPartialStore,
            OpaOptions opaOptions,
            IServiceProvider serviceProvider,
            ILogger<OpaWorker> logger)
        {
            _preparedPartialStore = preparedPartialStore;
            _opaOptions = opaOptions;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting OPADotNet");

            using var initActivity = new Activity("OPAInitialize")
            {
                DisplayName = "Initialize OPA"
            };
            initActivity.Start();

            var authPolicyProvider = _serviceProvider.GetService(typeof(IAuthorizationPolicyProvider));

            if (authPolicyProvider == null)
            {
                throw new InvalidOperationException("Cannot add OPA without adding AddAuthorization in services");
            }

            initActivity.AddEvent(new ActivityEvent("GetRequirements"));
            _logger.LogTrace("Getting OPA requirements");

            var requirements = RequirementsStore.GetRequirements();

            var embeddedClient = _serviceProvider.GetService(typeof(OpaClientEmbedded)) as OpaClientEmbedded;

            if (_opaOptions.UseEmbedded)
            {
                _logger.LogTrace("Initializing OPA discovery and policy synchronization");

                var initEvent = new ActivityEvent("InititializeDiscoveryAndPolicySync");
                initActivity.AddEvent(initEvent);
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

                var doneEvent = new ActivityEvent("InititializedDiscoveryAndPolicySync");
                initActivity.AddEvent(doneEvent);
                _logger.LogTrace($"Initialized OPA discovery and policy synchronization, time: {doneEvent.Timestamp.Subtract(initEvent.Timestamp).TotalMilliseconds}ms");
                
            }

            _logger.LogTrace("Preparing requirement queries");
            var beforePrepareRequirementsEvent = new ActivityEvent("PrepareRequirements");
            initActivity.AddEvent(beforePrepareRequirementsEvent);
            foreach(var requirement in requirements)
            {
                _preparedPartialStore.PreparePartial(requirement);
            }
            var afterPrepareRequirementsEvent = new ActivityEvent("PreparedRequirements");
            initActivity.AddEvent(afterPrepareRequirementsEvent);
            _logger.LogTrace($"Prepared requirement queries, elapsed time: {afterPrepareRequirementsEvent.Timestamp.Subtract(beforePrepareRequirementsEvent.Timestamp).TotalMilliseconds}ms");

            initActivity.Stop();
            _logger.LogInformation($"OPA initialized, elapsed time: {initActivity.Duration.TotalMilliseconds}ms");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
