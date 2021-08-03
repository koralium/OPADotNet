using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class DiscoverySyncBuilder : IDiscoverySyncBuilder
    {
        private readonly List<SyncServiceHolder> _syncServices = new List<SyncServiceHolder>();

        public string Decision { get; set; } = "data";

        public IServiceCollection Services { get; }

        public DiscoverySyncBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ISyncBuilder AddSyncService<T>(T service) where T : ISyncService
        {
            _syncServices.Add(new SyncServiceHolderObject(service));
            return this;
        }

        public ISyncBuilder AddSyncService<T>() where T : ISyncService
        {
            return AddSyncService(typeof(T));
        }

        public ISyncBuilder AddSyncService(Type type)
        {
            if (!typeof(ISyncService).IsAssignableFrom(type))
            {
                throw new ArgumentException($"'{type.Name}' does not implement the ISyncService interface", nameof(type));
            }
            _syncServices.Add(new SyncServiceHolderType(type));
            return this;
        }

        internal DiscoveryOptions Build()
        {
            if (_syncServices.Count > 1)
            {
                throw new InvalidOperationException("Only one sync service can be used for discovery");
            }
            return new DiscoveryOptions(_syncServices.FirstOrDefault(), Decision);
        }
    }
}
