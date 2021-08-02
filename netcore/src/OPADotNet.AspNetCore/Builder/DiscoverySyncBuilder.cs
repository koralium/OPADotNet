using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class DiscoverySyncBuilder : IDiscoverySyncBuilder
    {
        public string Decision { get; set; } = "data";

        public IServiceCollection Services { get; }

        public DiscoverySyncBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ISyncBuilder AddSyncService<T>(T service) where T : ISyncService
        {
            return this;
        }

        public ISyncBuilder AddSyncService<T>() where T : ISyncService
        {
            return this;
        }

        public ISyncBuilder AddSyncService(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
