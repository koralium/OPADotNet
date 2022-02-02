using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstWith : AstNode
    {
        [JsonPropertyName("target")]
        public AstTerm Target { get; set; }

        [JsonPropertyName("value")]
        public AstTerm Value { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitWith(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstWith other)
            {
                return Equals(Target, other.Target) &&
                    Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Target);
            hashCode.Add(Value);
            return hashCode.ToHashCode();
        }
    }
}
