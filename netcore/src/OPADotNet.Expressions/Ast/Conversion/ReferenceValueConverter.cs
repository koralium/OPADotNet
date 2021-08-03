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
    internal class ReferenceValueConverter : AstVisitor<ReferenceValue>
    {
        public override ReferenceValue VisitTermVar(AstTermVar partialTermVar)
        {
            bool isIterator = partialTermVar.Value.StartsWith("$") || partialTermVar.Value.StartsWith("__local");
            return new VariableReference()
            {
                Value = partialTermVar.Value,
                IsIterator = isIterator
            };
        }

        public override ReferenceValue VisitTermString(AstTermString partialTermString)
        {
            return new StringReference()
            {
                Value = partialTermString.Value
            };
        }

        public override ReferenceValue VisitTermRef(AstTermRef partialTermRef)
        {
            throw new InvalidOperationException("References cannot contain other references when converting to expressions");
        }
    }
}
