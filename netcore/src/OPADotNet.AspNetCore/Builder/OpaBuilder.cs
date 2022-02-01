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
using Microsoft.Extensions.DependencyInjection;
using OPADotNet.AspNetCore.Reasons;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using OPADotNet.Reasons;
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
        private DiscoveryOptions _discoveryOptions;
        private readonly SyncBuilder _syncBuilder;

        private bool _syncAdded = false;
        private bool _useReasons = true;

        internal OpaBuilder(IServiceCollection services)
        {
            Services = services;
            _syncBuilder = new SyncBuilder(Services);
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
            if (_syncAdded)
            {
                throw new InvalidOperationException("AddSync can only be called once");
            }
            builder?.Invoke(_syncBuilder);
            _syncAdded = true;
            return this;
        }

        public OpaBuilder UseReasons(bool useReasons = true)
        {
            _useReasons = useReasons;
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

            if (_useReasons)
            {
                _syncBuilder.AddSyncDoneHandler<ReasonCompileHandler>();
                Services.AddSingleton<IReasonHandler, ReasonHandler>();
                Services.AddSingleton<IReasonFormatter, DefaultReasonFormatter>();
            }

            var syncOptions = _syncBuilder.Build();

            if (_opaServerUrl != null && _useEmbedded)
            {
                throw new InvalidOperationException("Cannot run both embedded mode and Opa server mode.");
            }

            if (_opaServerUrl != null && (syncOptions.SyncServices.Count > 0))
            {
                throw new InvalidOperationException("Cannot run both policy and data sync with opa server mode.");
            }

            //We always use embedded mode if OPA server is not entered.
            bool embeddedMode = _opaServerUrl == null;

            if (_useReasons && !embeddedMode)
            {
                throw new InvalidOperationException("Reasons only work in embedded mode.");
            }

            return new OpaOptions(url, embeddedMode, syncOptions, _discoveryOptions);
        }
    }
}
