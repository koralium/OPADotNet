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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authorization
{
    public static class IAuthorizationServiceExtensions
    {
        /// <summary>
        /// Filters an IQueryable to only display the data that a user has the right to see.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="authorizationService"></param>
        /// <param name="data">The queryable to filter</param>
        /// <returns></returns>
        public static async Task<(AuthorizationResult AuthenticationResult, IQueryable<T> Data)> AuthorizeQueryable<T>(this IAuthorizationService authorizationService,ClaimsPrincipal user, IQueryable<T> data, string policyName)
        {
            var (authResult, filter) = await GetOpaFilterExpression<T>(authorizationService, user, policyName);

            if (authResult.Succeeded)
            {
                return (authResult, data.Where(filter));
            }

            return (authResult, Enumerable.Empty<T>().AsQueryable());
        }

        public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, ClaimsPrincipal user, object inputResource, object dataObject, string policyName)
        {
            var holder = new AuthorizeResourceDataHolder(inputResource, dataObject);
            return await authorizationService.AuthorizeAsync(user, holder, policyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="authorizationService"></param>
        /// <param name="user"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static async Task<(AuthorizationResult AuthenticationResult, Expression<Func<T, bool>> Expression)> GetOpaFilterExpression<T>(this IAuthorizationService authorizationService, ClaimsPrincipal user, string policyName)
        {
            var holder = new AuthorizeQueryableHolder(typeof(T), Expression.Parameter(typeof(T)));

            var authorizeResult = await authorizationService.AuthorizeAsync(user, holder, policyName);
            return (authorizeResult, holder.GetLambdaExpression() as Expression<Func<T, bool>>);
        }
    }
}
