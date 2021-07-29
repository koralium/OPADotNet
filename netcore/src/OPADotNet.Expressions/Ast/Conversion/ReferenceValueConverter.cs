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
