using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Expressions.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Conversion
{
    internal class ScalarExpressionConverter : AstVisitor<ScalarExpression>
    {
        private static readonly ReferenceValueConverter referenceValueConverter = new ReferenceValueConverter();
        public override ScalarExpression VisitTermString(AstTermString partialTermString)
        {
            return new StringLiteral()
            {
                Value = partialTermString.Value
            };
        }

        public override ScalarExpression VisitTermNumber(AstTermNumber partialTermNumber)
        {
            return new NumericLiteral()
            {
                Value = partialTermNumber.Value
            };
        }

        public override ScalarExpression VisitTermBoolean(AstTermBoolean termBoolean)
        {
            return new BoolLiteral()
            {
                Value = termBoolean.Value
            };
        }

        public override ScalarExpression VisitTermRef(AstTermRef partialTermRef)
        {
            List<ReferenceValue> references = new List<ReferenceValue>(partialTermRef.Value.Count);

            for (int i = 0; i < partialTermRef.Value.Count; i++)
            {
                var val = referenceValueConverter.Visit(partialTermRef.Value[i]);

                if (val == null)
                {
                    throw new InvalidOperationException("Could not convert a reference correctly.");
                }

                references.Add(val);
            }

            return new Reference()
            {
                References = references
            };
        }
    }
}
