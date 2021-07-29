
using System;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstRuleHead : AstNode
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public AstTerm Value { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitRuleHead(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstRuleHead other)
            {
                return Equals(Name, other.Name) &&
                    Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value);
        }
    }
}
