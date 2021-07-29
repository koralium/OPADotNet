using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermNumber : AstTerm
    {
        public decimal Value { get; set; }

        public override AstTermType Type => AstTermType.Number;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermNumber(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermNumber other)
            {
                return Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
