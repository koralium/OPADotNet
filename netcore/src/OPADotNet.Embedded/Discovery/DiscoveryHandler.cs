﻿using Microsoft.Extensions.Logging;
using OPADotNet.Embedded.Models;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using OPADotNet.Embedded.TarGzSync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Discovery
{
    /// <summary>
    /// Handles discovering configuration, and setting up sync that was found in the discovery.
    /// </summary>
    internal class DiscoveryHandler
    {
        private readonly List<SyncPolicyDescriptor> _syncPolicyDescriptors;
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscoveryOptions _discoveryOptions;
        private readonly ILogger _logger;
        private readonly ILogger<RestTarGzSync> _syncLogger;
        private readonly Dictionary<RestTarGzOptions, DiscoveryBundleContainer> _bundles = new Dictionary<RestTarGzOptions, DiscoveryBundleContainer>();
        private PreparedEvalEmbedded _discoveryEval;
        private SyncHandler _syncHandler;
        private ISyncService _discoverySyncService;
        private DiscoverySyncContext _discoverySyncContext;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _discoveryServiceTask;

        public DiscoveryHandler(
            List<SyncPolicyDescriptor> syncPolicyDescriptors,
            IServiceProvider serviceProvider
            )
        {
            _syncPolicyDescriptors = syncPolicyDescriptors;
            _serviceProvider = serviceProvider;
            _discoveryOptions = _serviceProvider.GetService(typeof(DiscoveryOptions)) as DiscoveryOptions;
            _logger = _serviceProvider.GetService(typeof(ILogger<DiscoveryHandler>)) as ILogger;
            _syncLogger = _serviceProvider.GetService(typeof(ILogger<RestTarGzSync>)) as ILogger<RestTarGzSync>;
        }

        private HashSet<RestTarGzOptions> ParseConfiguration(DiscoveryModel discoveryModel)
        {
            HashSet<RestTarGzOptions> bundles = new HashSet<RestTarGzOptions>();
            foreach(var bundle in discoveryModel.Result.Bundles)
            {
                if (!discoveryModel.Result.Services.TryGetValue(bundle.Value.Service, out var service))
                {
                    _logger.LogError($"Could not find any service named {bundle.Value.Service} while trying to configure bundle '{bundle.Key}'");
                }
                var url = service.Url;
                var restOptions = new RestTarGzOptions()
                {
                    Url = new Uri(new Uri(url), bundle.Value.Resource),
                    Interval = TimeSpan.FromSeconds(bundle.Value.Polling?.MinDelaySeconds ?? bundle.Value.Polling.MaxDelaySeconds ?? 60)
                };

                if (service.Headers != null)
                {
                    foreach(var header in service.Headers)
                    {
                        restOptions.Headers.Add(header.Key, header.Value);
                    }
                }

                if (service?.Credentials?.OAuth2 != null)
                {
                    var oauth2 = service.Credentials.OAuth2;
                    restOptions.CredentialMethod = new OAuth2CredentialMethod()
                    {
                        ClientId = oauth2.ClientId,
                        ClientSecret = oauth2.ClientSecret,
                        Scopes = oauth2.Scopes,
                        TokenUrl = oauth2.TokenUrl
                    };
                }

                bundles.Add(restOptions);
            }
            return bundles;
        }

        /// <summary>
        /// Update the current configuration
        /// </summary>
        internal async Task Update()
        {
            if (_discoveryEval == null)
            {
                return;
            }

            var discoveryData = await _discoveryEval.Evaluate<DiscoveryModel>(null);

            //Parse the configuration
            var bundleConfigurations = ParseConfiguration(discoveryData);

            var toDeleteList = _bundles.Keys.Except(bundleConfigurations);

            foreach(var toDelete in toDeleteList)
            {
                _bundles.Remove(toDelete);
            }

            foreach (var bundle in bundleConfigurations)
            {
                var service = new RestTarGzSync(bundle);
                if (!_bundles.ContainsKey(bundle))
                {
                    _bundles.Add(bundle, new DiscoveryBundleContainer()
                    {
                        RestTarGzOptions = bundle,
                        SyncServiceHolder = new SyncServiceHolderObject(service)
                    });
                }
            }

            var syncOptions = _serviceProvider.GetService(typeof(SyncOptions)) as SyncOptions;
            List<SyncServiceHolder> modifiedServices = new List<SyncServiceHolder>();

            foreach (var service in syncOptions.SyncServices)
            {
                modifiedServices.Add(service);
            }
            //Merge the existing sync services with the found from discovery
            foreach (var bundleService in _bundles.Values)
            {
                modifiedServices.Add(bundleService.SyncServiceHolder);
            }

            //Send updated service list to the sync handler
            await _syncHandler.UpdateSyncServices(modifiedServices);
        }

        public async Task Start()
        {
            _logger.LogTrace("Starting discovery handler");

            if (_discoveryOptions?.DiscoveryService != null)
            {
                _discoverySyncService = _discoveryOptions.DiscoveryService.GetService(_serviceProvider);

                //Create a new embedded client to be used for the discovery
                OpaClientEmbedded discoveryClient = new OpaClientEmbedded();
                _discoverySyncContext = new DiscoverySyncContext(discoveryClient, this);
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
                await _discoverySyncService.FullLoad(_discoverySyncContext, cancellationTokenSource.Token);
                discoveryActivity.Stop();

                if (_logger != null)
                {
                    _logger.LogInformation($"Discovery data loaded, elapsed time: {discoveryActivity.Duration}");
                }

                _discoveryEval = discoveryClient.PrepareEvaluation($"result = {_discoveryOptions.Decision}");
                var discoveryData = await _discoveryEval.Evaluate<DiscoveryModel>(null);

                //Parse the configuration
                var bundleConfigurations = ParseConfiguration(discoveryData);

                foreach(var bundle in bundleConfigurations)
                {
                    var service = new RestTarGzSync(bundle);
                    _bundles.Add(bundle, new DiscoveryBundleContainer()
                    {
                        RestTarGzOptions = bundle,
                        SyncServiceHolder = new SyncServiceHolderObject(service)
                    });
                }
            }
            
            var syncOptions = _serviceProvider.GetService(typeof(SyncOptions)) as SyncOptions;

            List<SyncServiceHolder> modifiedServices = new List<SyncServiceHolder>();

            if (syncOptions != null)
            {
                foreach (var service in syncOptions.SyncServices)
                {
                    modifiedServices.Add(service);
                }
            }

            //Merge the existing sync services with the found from discovery
            foreach(var bundleService in _bundles.Values)
            {
                modifiedServices.Add(bundleService.SyncServiceHolder);
            }

            var newOptions = new SyncOptions(modifiedServices);
            _syncHandler = new SyncHandler(newOptions, _syncPolicyDescriptors, _serviceProvider);
            await _syncHandler.Start();


            if (_discoverySyncService != null)
            {
                //If there is a discovery service, starts its background task after the first sync has been made.
                _discoveryServiceTask = Task.Factory.StartNew(async () =>
                {
                    await _discoverySyncService.BackgroundRun(_discoverySyncContext, _cancellationTokenSource.Token);
                }, TaskCreationOptions.LongRunning).Unwrap();
            }
        }
    }
}
