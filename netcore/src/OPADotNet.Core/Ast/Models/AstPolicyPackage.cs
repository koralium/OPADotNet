using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstPolicyPackage : AstNode
    {
        [JsonPropertyName("path")]
        public List<AstTerm> Path { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitPolicyPackage(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstPolicyPackage other)
            {
                return Path.AreEqual(other.Path);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var term in Path)
            {
                hashCode.Add(term);
            }
            return hashCode.ToHashCode();
        }
    }
}
