using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using OPADotNet.Automapper.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Automapper
{
    internal class PolicyValueResolver<TSource, TDestination, TSourceMember> : IMemberValueResolver<TSource, TDestination, TSourceMember, bool>
    {
        private readonly string _policyName;
        private readonly bool _useDestination;

        public PolicyValueResolver(string policyName, bool useDestination)
        {
            _policyName = policyName;
            _useDestination = useDestination;
        }

        public bool Resolve(TSource source, TDestination destination, TSourceMember sourceMember, bool destMember, ResolutionContext context)
        {
            var authService = context.Options.ServiceCtor(typeof(IAuthorizationService)) as IAuthorizationService;
            var userProvider = context.Options.ServiceCtor(typeof(IOpaAutoMapperUserProvider)) as IOpaAutoMapperUserProvider;

            if (authService == null)
            {
                throw new InvalidOperationException("Could not get IAuthorizationService.");
            }
            if (userProvider == null)
            {
                throw new InvalidOperationException("Could not get IOpaAutoMapperUserProvider.");
            }

            if (_useDestination)
            {
                return AsyncHelper.RunSync(() => authService.AuthorizeAsync(userProvider.GetClaimsPrincipal(), destination, _policyName)).Succeeded;
            }
            else
            {
                return AsyncHelper.RunSync(() => authService.AuthorizeAsync(userProvider.GetClaimsPrincipal(), source, _policyName)).Succeeded;
            }
        }
    }
}
