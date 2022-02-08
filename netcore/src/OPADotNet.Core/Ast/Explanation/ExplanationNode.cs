using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public abstract class ExplanationNode
    {
        [JsonPropertyName("op")]
        public string Operation { get; set; }

        [JsonPropertyName("query_id")]
        public int QueryId { get; set; }

        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }

        public abstract ExplanationType Type { get; }

        private protected abstract AstNode GetNode();

        [JsonPropertyName("node")]
        public AstNode Node => GetNode();

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("locals")]
        public List<ExplanationBinding> Locals { get; set; }

        [JsonPropertyName("location")]
        public ExplanationLocation Location { get; set; }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Operation);
            hash.Add(QueryId);
            hash.Add(ParentId);
            hash.Add(Type);
            hash.Add(Node);
            hash.Add(Message);
            hash.Add(Location);
            hash.Add(Type);
            hash.Add(Node);

            foreach (var local in Locals)
            {
                hash.Add(local);
            }
            return hash.ToHashCode();
        }
    }
}
