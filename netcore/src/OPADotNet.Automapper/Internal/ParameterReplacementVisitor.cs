using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OPADotNet.Automapper.Internal
{
    class ParameterReplacementVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplacementVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression parameterExpression && parameterExpression.Equals(_from))
            {
                return Expression.MakeMemberAccess(_to, node.Member);
            }
            return base.VisitMember(node);
        }
    }
}
