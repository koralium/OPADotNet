using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermBoolean : AstTerm
    {
        public override AstTermType Type => AstTermType.Boolean;

        public bool Value { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermBoolean(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermBoolean other)
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
