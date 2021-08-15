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
        private IPreparedEvaluation _discoveryEval;
        private readonly SyncHandler _syncHandler;
        private ISyncService _discoverySyncService;
        private DiscoverySyncContext _discoverySyncContext;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _discoveryServiceTask;

        public DiscoveryHandler(
            IServiceProvider serviceProvider,
            ILogger<DiscoveryHandler> logger,
            ILogger<RestTarGzSync> syncLogger,
            SyncHandler syncHandler
            )
        {
            _syncPolicyDescriptors = new List<SyncPolicyDescriptor>();
            _serviceProvider = serviceProvider;
            _discoveryOptions = serviceProvider.GetService(typeof(DiscoveryOptions)) as DiscoveryOptions;
            _logger = logger;
            _syncLogger = syncLogger;
            _syncHandler = syncHandler;
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
            var bundleConfigurations = ParseConfiguration(discoveryData.First());

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

        public async Task Start(IEnumerable<SyncPolicyDescriptor> syncPolicyDescriptors)
        {
            _logger.LogTrace("Starting discovery handler");
            _syncPolicyDescriptors.AddRange(syncPolicyDescriptors);

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
                var bundleConfigurations = ParseConfiguration(discoveryData.First());

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

            await _syncHandler.Start(_syncPolicyDescriptors, modifiedServices);

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
