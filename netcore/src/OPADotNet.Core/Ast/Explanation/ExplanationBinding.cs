using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public class ExplanationBinding
    {
        [JsonPropertyName("key")]
        public AstTerm Key { get; set; }

        [JsonPropertyName("value")]
        public AstTerm Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ExplanationBinding other)
            {
                return Equals(Key, other.Key) &&
                    Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }
    }
}
