using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OPADotNet.Automapper.Internal
{
    class ContainsOpaFilterVisitor : ExpressionVisitor
    {
        private bool _containsOpaFilter;

        public bool ContainsOpaFilter => _containsOpaFilter;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "FromPolicy" || node.Method.Name == "FromPolicyBasedOnDestination")
            {
                _containsOpaFilter = true;
                return node;
            }
            return base.VisitMethodCall(node);
        }
    }
}
