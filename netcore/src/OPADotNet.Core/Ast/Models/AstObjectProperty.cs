using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstObjectProperty : AstNode
    {
        public List<AstTerm> Values { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitObjectProperty(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstObjectProperty other)
            {
                return Values.AreEqual(other.Values);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var value in Values)
            {
                hashCode.Add(value);
            }
            return hashCode.ToHashCode();
        }
    }
}
