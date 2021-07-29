using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class Query : Node
    {
        public IList<BooleanExpression> AndExpressions { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitQuery(this);
        }
    }
}
