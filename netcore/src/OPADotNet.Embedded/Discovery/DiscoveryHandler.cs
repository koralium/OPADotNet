using Microsoft.Extensions.Logging;
using OPADotNet.Embedded.Models;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Discovery
{
    /// <summary>
    /// Handles discovering configuration, and setting up sync etc.
    /// </summary>
    internal class DiscoveryHandler
    {
        private readonly List<SyncPolicyDescriptor> _syncPolicyDescriptors;
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscoveryOptions _discoveryOptions;
        private readonly ILogger _logger;

        public DiscoveryHandler(
            List<SyncPolicyDescriptor> syncPolicyDescriptors,
            IServiceProvider serviceProvider
            )
        {
            _syncPolicyDescriptors = syncPolicyDescriptors;
            _serviceProvider = serviceProvider;
            _discoveryOptions = _serviceProvider.GetService(typeof(DiscoveryOptions)) as DiscoveryOptions;
            _logger = _serviceProvider.GetService(typeof(ILogger<DiscoveryHandler>)) as ILogger;
        }

        public async Task Start()
        {
            if (_discoveryOptions.DiscoveryService != null)
            {
                var discoveryService = _discoveryOptions.DiscoveryService.GetService(_serviceProvider);

                //Create a new embedded client to be used for the discovery
                OpaClientEmbedded discoveryClient = new OpaClientEmbedded();
                DiscoverySyncContext discoverySyncContext = new DiscoverySyncContext(discoveryClient);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                if (_logger != null)
                {
                    _logger.LogInformation("Loading discovery service data");
                }
                
                using var discoveryActivity = new Activity("FetchOpaDiscoveryData")
                {
                    DisplayName = "Load OPA Discovery Data"
                };
                discoveryActivity.Start();
                //Load in the data from the discovery, this is done
                await discoveryService.FullLoad(discoverySyncContext, cancellationTokenSource.Token);
                discoveryActivity.Stop();

                if (_logger != null)
                {
                    _logger.LogInformation($"Discovery data loaded, elapsed time: {discoveryActivity.Duration}");
                }

                var preparedEval = discoveryClient.PrepareEvaluation($"result = {_discoveryOptions.Decision}");
                var discoveryData = await preparedEval.Evaluate<DiscoveryModel>(null);
            }

            var syncOptions = _serviceProvider.GetService(typeof(SyncOptions)) as SyncOptions;
            SyncHandler syncHandler = new SyncHandler(syncOptions, _syncPolicyDescriptors, _serviceProvider);
            await syncHandler.Start();
        }
    }
}
