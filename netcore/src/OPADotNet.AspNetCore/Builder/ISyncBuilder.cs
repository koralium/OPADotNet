using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    public interface ISyncBuilder
    {
        IServiceCollection Services { get; }

        ISyncBuilder AddSyncService<T>(T service) where T : ISyncService;

        ISyncBuilder AddSyncService<T>() where T : ISyncService;

        ISyncBuilder AddSyncService(Type type);
    }
}
