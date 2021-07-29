using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstExpression : AstNode
    {
        public int Index { get; set; }

        public List<AstTerm> Terms { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitExpression(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstExpression other)
            {
                return Equals(Index, other.Index) &&
                    Terms.AreEqual(other.Terms);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Index);

            foreach(var term in Terms)
            {
                hashCode.Add(term);
            }

            return hashCode.ToHashCode();
        }
    }
}
