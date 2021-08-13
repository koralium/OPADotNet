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
using Microsoft.Extensions.Logging;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    internal class SyncHandler
    {
        /// <summary>
        /// Container for each running sync service
        /// </summary>
        private class SyncContainer
        {
            /// <summary>
            /// The sync service that is running
            /// </summary>
            public ISyncService SyncService { get; set; }

            /// <summary>
            /// The task for the background run
            /// </summary>
            public Task Task { get; set; }

            /// <summary>
            /// Cancellation token source that helps close the running sync
            /// </summary>
            public CancellationTokenSource CancellationTokenSource { get; set; }
        }

        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly Dictionary<Guid, SyncContainer> _syncServices;
        private readonly List<SyncPolicyDescriptor> _syncPolicyDescriptors;
        private readonly IServiceProvider _serviceProvider;
        private SyncContext _syncContext;
        private readonly ILogger _logger;

        public SyncHandler(
            OpaClientEmbedded opaClientEmbedded,
            ILogger<SyncHandler> logger,
            IServiceProvider serviceProvider)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _syncServices = new Dictionary<Guid, SyncContainer>();
            _syncPolicyDescriptors = new List<SyncPolicyDescriptor>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new policy to be monitored, if it does not exist, it will add it and load all the data for the policy.
        /// This can be very inefective and time consuming if the policy does not exist beforehand.
        /// </summary>
        /// <param name="syncPolicyDescriptor"></param>
        /// <returns>If the policy was found or not</returns>
        public async Task<bool> LoadPolicy(SyncPolicyDescriptor syncPolicyDescriptor)
        {
            var policy = _syncPolicyDescriptors.FirstOrDefault(x => x.PolicyName == syncPolicyDescriptor.PolicyName && x.Unknown == syncPolicyDescriptor.Unknown);
            
            if (policy == null)
            {
                _syncPolicyDescriptors.Add(syncPolicyDescriptor);
                await FullLoad();
            }
            return syncPolicyDescriptor.Found;
            }
        
        /// <summary>
        /// Update the existing sync services, used by the discovery handler to update which sync services are active.
        /// </summary>
        /// <param name="syncServices"></param>
        public async Task UpdateSyncServices(IReadOnlyList<SyncServiceHolder> syncServices)
        {
            var toDeleteList = _syncServices.Keys.Except(syncServices.Select(x => x.Id)).ToList();

            foreach(var toDelete in toDeleteList)
            {
                if (_syncServices.TryGetValue(toDelete, out var container))
                {
                    container.CancellationTokenSource.Cancel();
                    await container.Task;
                }
            }

            var toAddGuids = syncServices.Select(x => x.Id).Except(_syncServices.Keys);

            foreach(var toAdd in toAddGuids)
            {
                var toAddService = syncServices.FirstOrDefault(x => x.Id == toAdd);
                var service = toAddService.GetService(_serviceProvider);
                await service.Initialize(_serviceProvider);
                var cancelToken = new CancellationTokenSource();

                //Added services after start only start the background run.
                _syncServices.Add(toAddService.Id, new SyncContainer()
                {
                    SyncService = service,
                    CancellationTokenSource = cancelToken,
                    Task = Task.Factory.StartNew(async () =>
                    {
                        await service.BackgroundRun(_syncContext, cancelToken.Token);
                    }, TaskCreationOptions.LongRunning)
                    .Unwrap()
                });
            }
        }

        private async Task FullLoad()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var policyStep = _syncContext.NewIteration();

            _logger.LogTrace("Start loading policy data");
            foreach (var syncService in _syncServices)
            {
                await syncService.Value.SyncService.LoadPolices(policyStep, cancellationTokenSource.Token);
            }

            var dataStep = policyStep.Next();

            _logger.LogTrace("Start loading data");
            foreach (var syncService in _syncServices)
            {
                await syncService.Value.SyncService.LoadData(dataStep, cancellationTokenSource.Token);
            }

            await dataStep.Done();
        }

        public async Task Start(List<SyncPolicyDescriptor> syncPolicyDescriptors, IReadOnlyList<SyncServiceHolder> syncServices)
        {
            _logger.LogTrace("Starting SyncHandler");

            _syncPolicyDescriptors.AddRange(syncPolicyDescriptors);

            _logger.LogTrace("Configuring SyncContext");
            _syncContext = new SyncContext(_syncPolicyDescriptors, _opaClientEmbedded);

            foreach(var syncServiceHolder in syncServices)
            {
                var syncService = syncServiceHolder.GetService(_serviceProvider);
                await syncService.Initialize(_serviceProvider);
                _syncServices.Add(syncServiceHolder.Id, new SyncContainer()
                {
                    SyncService = syncService,
                    CancellationTokenSource = new CancellationTokenSource()
                });
            }

            await FullLoad();

            foreach (var policy in _syncPolicyDescriptors)
            {
                if (!policy.Found)
                {
                    _logger.LogError($"Could not find any policy with the name: '{policy.PolicyName}'");
                }
            }

            _logger.LogTrace("Starting background workers");
            foreach(var syncService in _syncServices.Values)
            {
                var backgroundTask = Task.Factory.StartNew(async () =>
                {
                    await syncService.SyncService.BackgroundRun(_syncContext, syncService.CancellationTokenSource.Token);
                }, TaskCreationOptions.LongRunning)
                    .Unwrap();
                syncService.Task = backgroundTask;
            }
            
        }
    }
}
