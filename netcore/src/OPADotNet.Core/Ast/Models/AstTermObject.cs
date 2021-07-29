using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermObject : AstTerm
    {
        public List<AstObjectProperty> Value { get; set; }

        public override AstTermType Type => AstTermType.Object;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermObject(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermObject other)
            {
                return Value.AreEqual(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var property in Value)
            {
                hashCode.Add(property);
            }
            return hashCode.ToHashCode();
        }
    }
}
