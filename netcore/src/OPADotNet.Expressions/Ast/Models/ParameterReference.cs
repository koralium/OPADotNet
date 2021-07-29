using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class ParameterReference : ReferenceValue
    {
        public override ReferenceType Type => ReferenceType.Parameter;

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitParameterReference(this);
        }
    }
}
