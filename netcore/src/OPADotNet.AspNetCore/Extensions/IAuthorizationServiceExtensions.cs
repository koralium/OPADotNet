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
using OPADotNet.Expressions;
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
        private const string EntityFrameworkNamespace = "Microsoft.EntityFrameworkCore";

        /// <summary>
        /// Filters an IQueryable to only display the data that a user has the right to see.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="authorizationService"></param>
        /// <param name="data">The queryable to filter</param>
        /// <returns></returns>
        public static async Task<AuthorizeQueryableResult<T>> AuthorizeQueryable<T>(this IAuthorizationService authorizationService,ClaimsPrincipal user, IQueryable<T> data, string policyName)
        {
            ExpressionConversionOptions options = new ExpressionConversionOptions();

            // If it is entity framework, we skip null checks on references
            if (data.GetType().Namespace.StartsWith(EntityFrameworkNamespace))
            {
                options.IgnoreNotNullReferenceChecks = true;
            }

            var (authResult, filter) = await GetOpaFilterExpression<T>(authorizationService, user, policyName, options);

            if (authResult.Succeeded)
            {
                return new AuthorizeQueryableResult<T>(authResult, data.Where(filter));
            }

            return new AuthorizeQueryableResult<T>(authResult, Enumerable.Empty<T>().AsQueryable());
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
        public static async Task<(AuthorizationResult AuthenticationResult, Expression<Func<T, bool>> Expression)> GetOpaFilterExpression<T>(this IAuthorizationService authorizationService, ClaimsPrincipal user, string policyName, ExpressionConversionOptions options = null)
        {
            var holder = new AuthorizeQueryableHolder(typeof(T), Expression.Parameter(typeof(T)), options);

            var authorizeResult = await authorizationService.AuthorizeAsync(user, holder, policyName);
            return (authorizeResult, holder.GetLambdaExpression() as Expression<Func<T, bool>>);
        }

        public static Task<(AuthorizationResult AuthenticationResult, LambdaExpression)> GetOpaFilterExpression(this IAuthorizationService authorizationService, ClaimsPrincipal user, string policyName, Type objectType)
        {
            return authorizationService.GetOpaFilterExpression(user, policyName, Expression.Parameter(objectType));
        }

        public static async Task<(AuthorizationResult AuthenticationResult, LambdaExpression)> GetOpaFilterExpression(this IAuthorizationService authorizationService, ClaimsPrincipal user, string policyName, ParameterExpression parameterExpression, ExpressionConversionOptions options = null)
        {
            var holder = new AuthorizeQueryableHolder(parameterExpression.Type, parameterExpression, options);
            var authorizeResult = await authorizationService.AuthorizeAsync(user, holder, policyName);
            return (authorizeResult, holder.GetLambdaExpression());
        }
    }
}
