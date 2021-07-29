using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstBody : AstNode
    {
        public List<AstExpression> Expressions { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitBody(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstBody other)
            {
                return Expressions.AreEqual(other.Expressions);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach(var expression in Expressions)
            {
                hashCode.Add(expression);
            }
            return hashCode.ToHashCode();
        }
    }
}
