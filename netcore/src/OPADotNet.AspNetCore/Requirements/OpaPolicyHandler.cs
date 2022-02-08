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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.AspNetCore.Extensions;
using OPADotNet.AspNetCore.Input;
using OPADotNet.AspNetCore.Reasons;
using OPADotNet.Ast.Models;
using OPADotNet.Core.Models;
using OPADotNet.Embedded.sync;
using OPADotNet.Expressions;
using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore.Requirements
{
    class OpaPolicyHandler : AuthorizationHandler<OpaPolicyRequirement>
    {
        private readonly PreparedPartialStore _preparedPartialStore;
        private readonly ILogger<OpaPolicyHandler> _logger;
        private readonly ExpressionConverter _expressionConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SyncHandler _syncHandler;
        private readonly IReasonHandler _reasonHandler;
        private readonly IReasonFormatter _reasonFormatter;
        private readonly OpaOptions _opaOptions;

        public OpaPolicyHandler(
            PreparedPartialStore preparedPartialStore,
            ExpressionConverter expressionConverter,
            ILogger<OpaPolicyHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            OpaOptions opaOptions,
            SyncHandler syncHandler = default,
            IReasonHandler reasonHandler = default,
            IReasonFormatter reasonFormatter = default)
        {
            _opaOptions = opaOptions;
            _preparedPartialStore = preparedPartialStore;
            _logger = logger;
            _expressionConverter = expressionConverter;
            _httpContextAccessor = httpContextAccessor;
            _syncHandler = syncHandler;
            _reasonHandler = reasonHandler;
            _reasonFormatter = reasonFormatter;
        }

        private OpaInput GetInput(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            return AddContextInput(new OpaInput()
            {
                Subject = OpaInputUser.FromPrincipal(context.User),
                Extensions = new Dictionary<string, object>(),
                Operation = requirement.Operation
            });
        }

        private OpaInput AddContextInput(OpaInput opaInput)
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                return opaInput;

            OpaInputRequest opaInputRequest = new OpaInputRequest();
            opaInputRequest.Path = _httpContextAccessor.HttpContext.Request.Path.Value?.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
            opaInputRequest.RouteValues = _httpContextAccessor.HttpContext.Request.RouteValues;
            opaInputRequest.Query = _httpContextAccessor.HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToList());
            opaInputRequest.Method = _httpContextAccessor.HttpContext.Request.Method;

            opaInput.Request = opaInputRequest;

            return opaInput;
        }

        private string GenerateReason(string query, PartialResult result)
        {
            if (_reasonHandler != null)
            {
                if (_reasonHandler.TryGetReasonMessage(query, result.Explanation, out var reasonMessage))
                {
                    return _reasonFormatter.FormatReason(reasonMessage);
                }
            }
            return "You are not authorized.";
        }

        private async Task AuthorizeResource(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            var input = GetInput(context, requirement);
            //Add the resource as an input, it is up to the user if they want to validate it using input or data
            input.Extensions.Add(requirement.GetInputResourceName(), context.Resource);
            
            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, requirement.GetUnknowns(), _opaOptions.UseReasons);

            if (result.Result.Queries == null)
            {
#if NET
                context.Fail(new OpaAuthorizationFailureReason(this, GenerateReason(requirement.GetQuery(), result), result.Explanation));
#endif
                return;
            }

            var expr = await _expressionConverter.ToExpression(result.Result, requirement.GetUnknown(), context.Resource.GetType());

            var func = expr.Compile();

            if (func(context.Resource))
            {
                context.Succeed(requirement);
            }
        }

        private async Task AuthorizeQueryable(AuthorizationHandlerContext context, OpaPolicyRequirement requirement, AuthorizeQueryableHolder authorizeQueryableHolder)
        {
            var input = GetInput(context, requirement);

            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, requirement.GetUnknowns(), _opaOptions.UseReasons);

            if (result.Result.Queries == null)
            {
#if NET
                context.Fail(new OpaAuthorizationFailureReason(this, GenerateReason(requirement.GetQuery(), result), result.Explanation));
#endif
                return;
            }

            var expression = await _expressionConverter.ToExpression(result.Result, requirement.GetUnknown(), authorizeQueryableHolder.ParameterExpression, authorizeQueryableHolder.Options);
            authorizeQueryableHolder.AddFilter(expression);
            context.Succeed(requirement);
        }

        private async Task AuthorizeInputObjDataObj(AuthorizationHandlerContext context, OpaPolicyRequirement requirement, AuthorizeResourceDataHolder holder)
        {
            var input = GetInput(context, requirement);
            input.Extensions.Add(requirement.GetInputResourceName(), holder.Resource);

            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, requirement.GetUnknowns(), _opaOptions.UseReasons);

            if (result.Result.Queries == null)
            {
#if NET
                context.Fail(new OpaAuthorizationFailureReason(this, GenerateReason(requirement.GetQuery(), result), result.Explanation));
#endif
                return;
            }

            var expr = await _expressionConverter.ToExpression(result.Result, requirement.GetUnknown(), holder.Data.GetType());

            var func = expr.Compile();

            if (func(holder.Data))
            {
                context.Succeed(requirement);
            }
        }

        private async Task AuthorizeRequest(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            var input = GetInput(context, requirement);

            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, requirement.GetUnknowns(), _opaOptions.UseReasons);

            //context.Fail(new AuthorizationFailureReason(this, "Message"));
            if (result.Result.Queries != null)
            {
                context.Succeed(requirement);
            }
#if NET
            else
            {
                context.Fail(new OpaAuthorizationFailureReason(this, GenerateReason(requirement.GetQuery(), result), result.Explanation));
            }
#endif
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            if (_syncHandler != null)
            {
                //Check that the policy has been loaded
                await _syncHandler.LoadPolicy(requirement.ToSyncPolicy());
            }

            if (context.Resource != null && !(context.Resource is Microsoft.AspNetCore.Routing.RouteEndpoint)
               && !(context.Resource is AuthorizeQueryableHolder)
               && !(context.Resource is AuthorizeResourceDataHolder)
               && !(context.Resource is HttpContext))
            {
                //Resource authorization check
                await AuthorizeResource(context, requirement);
            }
            else if (context.Resource is AuthorizeQueryableHolder holder)
            {
                //Queryable, create an expression that can be run.
                await AuthorizeQueryable(context, requirement, holder);
            }
            else if (context.Resource is AuthorizeResourceDataHolder resourceDataHolder)
            {
                await AuthorizeInputObjDataObj(context, requirement, resourceDataHolder);
            }
            else
            {
                //Not a resource validation, only check that the user can call the endpoint.
                await AuthorizeRequest(context, requirement);
            }
        }
    }
}
