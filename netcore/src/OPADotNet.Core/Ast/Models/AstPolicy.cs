using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstPolicy : AstNode
    {
        [JsonPropertyName("package")]
        public AstPolicyPackage Package { get; set; }

        [JsonPropertyName("rules")]
        public List<AstPolicyRule> Rules { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitPolicy(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstPolicy other)
            {
                return Equals(Package, other.Package) &&
                    Rules.AreEqual(other.Rules);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Package);

            foreach (var rule in Rules)
            {
                hashCode.Add(rule);
            }

            return hashCode.ToHashCode();
        }
    }
}
