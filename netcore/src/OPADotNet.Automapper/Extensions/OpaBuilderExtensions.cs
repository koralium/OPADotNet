using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Automapper;
using OPADotNet.Automapper.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpaBuilderExtensions
    {
        public static OpaBuilder AddAutomapperSupport(this OpaBuilder opaBuilder)
        {
            opaBuilder.AddDecorate();

            //Add default user provider
            opaBuilder.Services.AddScoped<IOpaAutoMapperUserProvider, DefaultUserProvider>();

            return opaBuilder;
        }

        public static OpaBuilder AddAutomapperSupport<TUserProvider>(this OpaBuilder opaBuilder)
            where TUserProvider : class, IOpaAutoMapperUserProvider
        {
            opaBuilder.AddDecorate();

            //Add default user provider
            opaBuilder.Services.AddScoped<IOpaAutoMapperUserProvider, TUserProvider>();

            return opaBuilder;
        }

        private static OpaBuilder AddDecorate(this OpaBuilder opaBuilder)
        {
            if (!opaBuilder.Services.TryDecorate<IMapper, PolicyMapper>())
            {
                throw new InvalidOperationException("AutoMapper must be added to services before Opa AutoMapper support can be added.");
            }
            return opaBuilder;
        }
    }
}
