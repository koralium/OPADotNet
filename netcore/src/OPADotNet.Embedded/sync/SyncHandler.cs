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
        private readonly SyncOptions _syncOptions;
        private readonly Dictionary<Guid, SyncContainer> _syncServices;
        private readonly List<SyncPolicyDescriptor> _syncPolicyDescriptors;
        private readonly IServiceProvider _serviceProvider;
        private SyncContext _syncContext;

        public SyncHandler(
            SyncOptions syncOptions,
            //List<ISyncService> syncServices, 
            //List<Type> syncServiceTypes, 
            List<SyncPolicyDescriptor> syncPolicyDescriptors,
            IServiceProvider serviceProvider)
        {
            _syncOptions = syncOptions;
            _opaClientEmbedded = serviceProvider.GetService(typeof(OpaClientEmbedded)) as OpaClientEmbedded;
            _syncServices = new Dictionary<Guid, SyncContainer>();
            _syncPolicyDescriptors = syncPolicyDescriptors;
            _serviceProvider = serviceProvider;
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

        public async Task Start()
        {
            var logger = _serviceProvider.GetService(typeof(ILogger<SyncHandler>)) as ILogger;
            _syncContext = new SyncContext(_syncPolicyDescriptors, _opaClientEmbedded);

            foreach(var syncServiceHolder in _syncOptions.SyncServices)
            {
                _syncServices.Add(syncServiceHolder.Id, new SyncContainer()
                {
                    SyncService = syncServiceHolder.GetService(_serviceProvider),
                    CancellationTokenSource = new CancellationTokenSource()
                });
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var policyStep = _syncContext.NewIteration();
            foreach (var syncService in _syncServices)
            {
                await syncService.Value.SyncService.LoadPolices(policyStep, cancellationTokenSource.Token);
            }

            var dataStep = policyStep.Next();

            foreach (var syncService in _syncServices)
            {
                await syncService.Value.SyncService.LoadData(dataStep, cancellationTokenSource.Token);
            }

            await dataStep.Done();

            foreach (var policy in _syncPolicyDescriptors)
            {
                if (!policy.Found)
                {
                    logger.LogError($"Could not find any policy with the name: '{policy.PolicyName}'");
                }
            }

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
