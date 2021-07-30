using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    internal class SyncHandler
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<ISyncService> _syncServices;
        private readonly List<Type> _syncServiceTypes;
        private readonly List<SyncPolicyDescriptor> _syncPolicyDescriptors;
        private readonly IServiceProvider _serviceProvider;

        private readonly List<Task> _backgroundTasks = new List<Task>();

        public SyncHandler(
            OpaClientEmbedded opaClientEmbedded, 
            List<ISyncService> syncServices, 
            List<Type> syncServiceTypes, 
            List<SyncPolicyDescriptor> syncPolicyDescriptors,
            IServiceProvider serviceProvider)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _syncServices = syncServices;
            _syncServiceTypes = syncServiceTypes;
            _syncPolicyDescriptors = syncPolicyDescriptors;
            _serviceProvider = serviceProvider;
        }

        public async Task Start()
        {
            SyncContext syncContext = new SyncContext(_syncPolicyDescriptors, _opaClientEmbedded);

            foreach (var syncServiceType in _syncServiceTypes)
            {
                var syncService = _serviceProvider.GetService(syncServiceType) as ISyncService;

                if (syncService == null)
                {
                    throw new InvalidOperationException($"Could not find type '{syncServiceType.Name}' through dependency injection.");
                }

                _syncServices.Add(syncService);
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var policyStep = syncContext.NewIteration();
            foreach (var syncService in _syncServices)
            {
                await syncService.LoadPolices(policyStep, cancellationTokenSource.Token);
            }

            var dataStep = policyStep.Next();

            foreach (var syncService in _syncServices)
            {
                await syncService.LoadData(dataStep, cancellationTokenSource.Token);
            }

            dataStep.Done();

            foreach (var policy in _syncPolicyDescriptors)
            {
                if (!policy.Found)
                {
                    throw new InvalidOperationException($"Could not find any policy with the name: '{policy.PolicyName}'");
                }
            }

            foreach(var syncService in _syncServices)
            {
                var backgroundTask = Task.Factory.StartNew(async () =>
                {
                    await syncService.BackgroundRun(syncContext, cancellationTokenSource.Token);
                }, TaskCreationOptions.LongRunning)
                    .Unwrap();
                _backgroundTasks.Add(backgroundTask);
            }
            
        }
    }
}
