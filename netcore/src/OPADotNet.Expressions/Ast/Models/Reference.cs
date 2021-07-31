using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class Reference : ScalarExpression
    {
        public List<ReferenceValue> References { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitReference(this);
        }

        public override string ToString()
        {
            return string.Join(".", References.Select(x => x.Value));
        }
    }
}
