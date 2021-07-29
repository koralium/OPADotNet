using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class StringReference : ReferenceValue
    {
        public override ReferenceType Type => ReferenceType.String;

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitStringReference(this);
        }
    }
}
