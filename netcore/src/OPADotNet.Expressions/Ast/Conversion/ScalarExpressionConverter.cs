/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
