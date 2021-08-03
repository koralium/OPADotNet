using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    public class OpaBuilder
    {
        private string _opaServerUrl;
        private bool _useEmbedded = false;
        private SyncOptions _syncOptions;
        private DiscoveryOptions _discoveryOptions;

        internal OpaBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// Add a connection to an OPA server. This removes the built in embedded mode, which will reduce performance.
        /// </summary>
        /// <param name="url"></param>
        public OpaBuilder UseOpaServer(string url)
        {
            _opaServerUrl = url;
            return this;
        }

        public OpaBuilder AddSync(Action<ISyncBuilder> builder)
        {
            if (_syncOptions != null)
            {
                throw new InvalidOperationException("AddSync can only be called once");
            }

            SyncBuilder syncBuilder = new SyncBuilder(Services);
            builder?.Invoke(syncBuilder);
            _syncOptions = syncBuilder.Build();
            return this;
        }

        public OpaBuilder AddDiscoverySync(Action<IDiscoverySyncBuilder> builder)
        {
            if (_discoveryOptions != null)
            {
                throw new InvalidOperationException("AddDiscoverySync can only be called once");
            }

            DiscoverySyncBuilder discoverySyncBuilder = new DiscoverySyncBuilder(Services);
            builder?.Invoke(discoverySyncBuilder);
            _discoveryOptions = discoverySyncBuilder.Build();
            
            return this;

        }

        /// <summary>
        /// Should embedded mode be used? Default is true
        /// </summary>
        /// <param name="useEmbedded"></param>
        /// <returns></returns>
        public OpaBuilder UseEmbedded()
        {
            _useEmbedded = true;
            return this;
        }

        internal OpaOptions Build()
        {
            Uri url = null;
            if (_opaServerUrl != null)
            {
                url = new Uri(_opaServerUrl);
            }

            if (_opaServerUrl != null && _useEmbedded)
            {
                throw new InvalidOperationException("Cannot run both embedded mode and Opa server mode.");
            }

            if (_opaServerUrl != null && (_syncOptions.SyncServices.Count > 0))
            {
                throw new InvalidOperationException("Cannot run both policy and data sync with opa server mode.");
            }

            //We always use embedded mode if OPA server is not entered.
            bool embeddedMode = _opaServerUrl == null;

            return new OpaOptions(url, embeddedMode, _syncOptions, _discoveryOptions);
        }
    }
}
