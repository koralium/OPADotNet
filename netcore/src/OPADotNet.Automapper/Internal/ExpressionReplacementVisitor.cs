using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Authorization;
using OPADotNet.Automapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OPADotNet.Automapper
{
    internal class ExpressionReplacementVisitor : ExpressionVisitor
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly Type _destinationType;
        private readonly Stack<ParameterExpression> _parameters = new Stack<ParameterExpression>();
        private readonly IOpaAutoMapperUserProvider _userProvider;
        public ExpressionReplacementVisitor(IMapper mapper, IAuthorizationService authorizationService, IOpaAutoMapperUserProvider userProvider, Type destination)
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _destinationType = destination;
            _userProvider = userProvider;
        }

        private string GetPolicyName(MethodCallExpression node)
        {
            var policyNameExpression = node.Arguments.First();
            var partiallyEvaluated = PartialEvaluator.PartialEval(policyNameExpression);

            if (!(partiallyEvaluated is ConstantExpression constantExpression))
            {
                throw new InvalidOperationException("Error");
            }

            return constantExpression.Value as string;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Select" || node.Method.Name == "Where")
            {
                var args = Visit(node.Arguments);
                return node.Update(node.Object, args);
            }
            if (node.Method.Name == "FromPolicy")
            {
                var policyName = GetPolicyName(node);

                var (authResult, lambda) = AsyncHelper.RunSync(() => _authorizationService.GetOpaFilterExpression(_userProvider.GetClaimsPrincipal(), policyName, _parameters.Peek()));
                return lambda.Body;
            }
            else if (node.Method.Name == "FromPolicyBasedOnDestination")
            {
                var policyName = GetPolicyName(node);

                var t = Expression.GetDelegateType(_parameters.Peek().Type, typeof(bool));
                var genericTypeDest = typeof(Expression<>).MakeGenericType(t);

                var (authResult, lambda) = AsyncHelper.RunSync(() => _authorizationService.GetOpaFilterExpression(_userProvider.GetClaimsPrincipal(), policyName, _destinationType));

                //Convert from destination into source
                var mappedExpr = _mapper.MapExpression(lambda, lambda.GetType(), genericTypeDest);

                var replacedParametersExpr = new ParameterReplacementVisitor(mappedExpr.Parameters.First(), _parameters.Peek()).Visit(mappedExpr.Body);
                return replacedParametersExpr;
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _parameters.Push(node.Parameters.First());
            return base.VisitLambda(node);
        }
    }
}
