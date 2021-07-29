using Microsoft.AspNetCore.Authorization;
using OPADotNet.AspNetCore.Requirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationOptions"></param>
        /// <param name="policyName">Name of the policy</param>
        /// <param name="moduleName">Name of the module, this will translate to data.{moduleName}</param>
        /// <param name="dataName">Data name that will be used in partial queries, translated to data.{dataName}</param>
        public static void AddOpaPolicy(this AuthorizationOptions authorizationOptions, string policyName, string moduleName, string dataName)
        {
            authorizationOptions.AddPolicy(policyName, x => x.AddRequirements(new OpaPolicyRequirement(moduleName, dataName)));
        }
    }
}
