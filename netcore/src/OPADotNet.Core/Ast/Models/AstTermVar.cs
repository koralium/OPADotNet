using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermVar : AstTerm
    {
        public string Value { get; set; }

        public override AstTermType Type => AstTermType.Var;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermVar(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermVar other)
            {
                return Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
