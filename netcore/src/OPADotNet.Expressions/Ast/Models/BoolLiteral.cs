using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class BoolLiteral : Literal
    {
        public bool Value { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitBoolLiteral(this);
        }
    }
}
