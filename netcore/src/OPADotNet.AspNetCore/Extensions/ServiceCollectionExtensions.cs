using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OPADotNet;
using OPADotNet.AspNetCore;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.AspNetCore.Requirements;
using OPADotNet.Embedded;
using OPADotNet.Embedded.sync;
using OPADotNet.Expressions;
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

            if (opt.SyncOptions != null)
            {
                services.AddSingleton(opt.SyncOptions);
            }
            
            if (opt.DiscoveryOptions != null)
            {
                services.AddSingleton(opt.DiscoveryOptions);
            }

            RestOpaClient restOpaClient = null;

            services.AddSingleton<PreparedPartialStore>();
            services.AddSingleton<ExpressionConverter>();

            if (opt.OpaServer != null)
            {
                restOpaClient = new RestOpaClient(opt.OpaServer);
                services.AddSingleton(restOpaClient);
                services.AddSingleton<IOpaClient>(x => x.GetRequiredService<RestOpaClient>());
            }
            else if (opt.UseEmbedded)
            {
                if (opt.SyncOptions?.SyncServices != null)
                {
                    foreach (var syncServiceHolder in opt.SyncOptions?.SyncServices)
                    {
                        syncServiceHolder.AddToServices(services);
                    }
                }
                
                var embeddedClient = new OpaClientEmbedded();
                services.AddSingleton(embeddedClient.OpaStore);
                services.AddSingleton(embeddedClient);
                services.AddSingleton<IOpaClient>(x => x.GetRequiredService<OpaClientEmbedded>());
            }

            services.AddHostedService<FinalizeWorker>();

            services.AddSingleton<IAuthorizationHandler, OpaPolicyHandler>();

            return services;
        }
    }
}
