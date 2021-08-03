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
using OPADotNet.Partial.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Conversion
{
    internal class OperatorConverter : AstVisitor<BooleanComparisonType?>
    {
        public override BooleanComparisonType? VisitTermRef(AstTermRef partialTermRef)
        {
            if (partialTermRef.Value.Count != 1)
            {
                throw new InvalidOperationException("Could not find a boolean comparison term");
            }
            if (partialTermRef.Value[0] is AstTermVar termVar)
            {
                switch (termVar.Value)
                {
                    case "eq":
                        return BooleanComparisonType.Equals;
                    case "lte":
                        return BooleanComparisonType.LessThanOrEqualTo;
                    case "lt":
                        return BooleanComparisonType.LessThan;
                    case "gt":
                        return BooleanComparisonType.GreaterThan;
                    case "gte":
                        return BooleanComparisonType.GreaterThanOrEqualTo;
                    case "ne":
                        return BooleanComparisonType.NotEqualTo;
                }
            }
            return null;
        }
    }
}
