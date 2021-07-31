using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OPADotNet.Ast.Models;
using OPADotNet.Expressions;
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

        public OpaPolicyHandler(PreparedPartialStore preparedPartialStore, ExpressionConverter expressionConverter, ILogger<OpaPolicyHandler> logger)
        {
            _preparedPartialStore = preparedPartialStore;
            _logger = logger;
            _expressionConverter = expressionConverter;
        }

        private OpaInput GetInput(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            return new OpaInput()
            {
                Subject = OpaInputUser.FromPrincipal(context.User),
                Extensions = new Dictionary<string, object>(),
                Operation = requirement.Operation
            };
        }

        private async Task AuthorizeResource(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            var input = GetInput(context, requirement);
            //Add the resource as an input, it is up to the user if they want to validate it using input or data
            input.Extensions.Add(requirement.GetInputResourceName(), context.Resource);

            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, new List<string>()
            {
                requirement.GetUnknown()
            });

            if (result.Queries == null)
            {
                return;
            }

            var expr = await _expressionConverter.ToExpression(result, requirement.GetUnknown(), context.Resource.GetType());

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
            var result = await preparedPartial.Partial(input, new List<string>()
            {
                requirement.GetUnknown()
            });

            if (result.Queries == null)
            {
                return;
            }

            var expression = await _expressionConverter.ToExpression(result, requirement.GetUnknown(), authorizeQueryableHolder.ParameterExpression);
            authorizeQueryableHolder.AddFilter(expression);
            context.Succeed(requirement);
        }

        private async Task AuthorizeInputObjDataObj(AuthorizationHandlerContext context, OpaPolicyRequirement requirement, AuthorizeResourceDataHolder holder)
        {
            var input = GetInput(context, requirement);
            input.Extensions.Add(requirement.GetInputResourceName(), holder.Resource);

            var preparedPartial = _preparedPartialStore.GetPreparedPartial(requirement);
            var result = await preparedPartial.Partial(input, new List<string>()
            {
                requirement.GetUnknown()
            });

            if (result.Queries == null)
            {
                return;
            }

            var expr = await _expressionConverter.ToExpression(result, requirement.GetUnknown(), holder.Data.GetType());

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
            var result = await preparedPartial.Partial(input, new List<string>()
            {
                requirement.GetUnknown()
            });

            if (result.Queries != null)
            {
                context.Succeed(requirement);
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            if (!(context.Resource is Microsoft.AspNetCore.Routing.RouteEndpoint) && !(context.Resource is AuthorizeQueryableHolder) && !(context.Resource is AuthorizeResourceDataHolder))
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
