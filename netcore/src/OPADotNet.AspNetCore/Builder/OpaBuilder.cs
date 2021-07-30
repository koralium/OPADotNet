using Microsoft.Extensions.DependencyInjection;
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
        private readonly List<ISyncService> _syncServices = new List<ISyncService>();
        private readonly List<Type> _syncServiceTypes = new List<Type>();

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

        public OpaBuilder AddSyncService<T>(T service) where T : ISyncService
        {
            _syncServices.Add(service);
            return this;
        }

        public OpaBuilder AddSyncService<T>() where T: ISyncService
        {
            _syncServiceTypes.Add(typeof(T));
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

            if (_opaServerUrl != null && (_syncServices.Count > 0 || _syncServiceTypes.Count > 0))
            {
                throw new InvalidOperationException("Cannot run both policy and data sync with opa server mode.");
            }

            //We always use embedded mode if OPA server is not entered.
            bool embeddedMode = _opaServerUrl == null;

            return new OpaOptions(url, embeddedMode, _syncServices.ToList(), _syncServiceTypes.ToList());
        }
    }
}
