/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationPolicyBuilder"></param>
        /// <param name="policyName">Name of the policy, translated to data.{policyName}.allow == true in the query</param>
        /// <param name="dataName">Name of the unknown data.</param>
        /// <param name="operation">Operation, this will be available in input.operation</param>
        /// <returns></returns>
        public static AuthorizationPolicyBuilder RequireOpaPolicy(this AuthorizationPolicyBuilder authorizationPolicyBuilder, string policyName, string dataName, string operation, Action<OpaPolicyRequirementOptions> options = null)
        {
            OpaPolicyRequirementOptions opaPolicyRequirementOptions = new OpaPolicyRequirementOptions();
            options?.Invoke(opaPolicyRequirementOptions);
            var requirement = new OpaPolicyRequirement(policyName, dataName, operation, opaPolicyRequirementOptions);
            authorizationPolicyBuilder.Requirements.Add(requirement);
            RequirementsStore.AddRequirement(requirement);
            return authorizationPolicyBuilder;
        }
    }
}
