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
