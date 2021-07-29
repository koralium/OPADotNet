using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class AstQueries : AstNode
    {
        [JsonPropertyName("queries")]
        public List<AstBody> Queries { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitQueries(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstQueries other)
            {
                return Queries.AreEqual(other.Queries);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var query in Queries)
            {
                hashCode.Add(query);
            }
            return hashCode.ToHashCode();
        }
    }
}
