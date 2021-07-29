
using System;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstPolicyRule : AstNode
    {
        [JsonPropertyName("head")]
        public AstRuleHead Head { get; set; }

        [JsonPropertyName("body")]
        public AstBody Body { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitPolicyRule(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstPolicyRule other)
            {
                return Equals(Head, other.Head) &&
                    Equals(Body, other.Body);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Head, Body);
        }
    }
}
