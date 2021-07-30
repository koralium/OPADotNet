﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OPADotNet;
using OPADotNet.AspNetCore;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.AspNetCore.Requirements;
using OPADotNet.Embedded;
using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpa(this IServiceCollection services, Action<OpaBuilder> options)
        {
            var builder = new OpaBuilder(services);
            options?.Invoke(builder);
            var opt = builder.Build();

            services.AddSingleton(opt);

            RestOpaClient restOpaClient = null;

            services.AddSingleton<PreparedPartialStore>();

            if (opt.OpaServer != null)
            {
                restOpaClient = new RestOpaClient(opt.OpaServer);
                services.AddSingleton(restOpaClient);
                services.AddSingleton<IOpaClient>(x => x.GetRequiredService<RestOpaClient>());
            }
            else if (opt.UseEmbedded)
            {
                foreach(var serviceType in opt.SyncServiceTypes)
                {
                    services.AddSingleton(serviceType);
                }
                var store = new OpaStore();
                services.AddSingleton(store);
                var embeddedClient = new OpaClientEmbedded(store);
                services.AddSingleton(embeddedClient);
                services.AddSingleton<IOpaClient>(x => x.GetRequiredService<OpaClientEmbedded>());
            }

            services.AddHostedService<FinalizeWorker>();

            services.AddSingleton<IAuthorizationHandler, OpaPolicyHandler>();

            return services;
        }
    }
}
