using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class VariableReference : ReferenceValue
    {
        public override ReferenceType Type => ReferenceType.Variable;

        public bool IsIterator { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitVariableReference(this);
        }
    }
}
