using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    class NullLiteral : Literal
    {
        public static readonly NullLiteral Defult = new NullLiteral();

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitNullLiteral(this);
        }
    }
}
