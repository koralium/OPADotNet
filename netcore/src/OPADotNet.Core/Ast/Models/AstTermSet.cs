using OPADotNet.Ast.Models;
using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermSet : AstTerm
    {
        public override AstTermType Type => AstTermType.Set;

        public List<AstTerm> Value { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermSet(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermSet other)
            {
                return Value.AreEqual(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var term in Value)
            {
                hashCode.Add(term);
            }
            return hashCode.ToHashCode();
        }
    }
}
