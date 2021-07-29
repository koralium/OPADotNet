using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class AnyCall : BooleanExpression
    {
        public Reference Property { get; set; }

        public string ParameterName { get; set; }

        public List<BooleanExpression> AndExpressions { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitAnyCall(this);
        }
    }
}
