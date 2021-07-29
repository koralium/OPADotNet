using Microsoft.AspNetCore.Authorization;
using OPADotNet.AspNetCore;
using OPADotNet.AspNetCore.Requirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireOpaPolicy(this AuthorizationPolicyBuilder authorizationPolicyBuilder, string policyName, string dataName)
        {
            var requirement = new OpaPolicyRequirement(policyName, dataName);
            authorizationPolicyBuilder.Requirements.Add(requirement);
            RequirementsStore.AddRequirement(requirement);
            return authorizationPolicyBuilder;
        }
    }
}
