using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermString : AstTerm
    {
        public string Value { get; set; }

        public override AstTermType Type => AstTermType.String;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermString(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermString other)
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
