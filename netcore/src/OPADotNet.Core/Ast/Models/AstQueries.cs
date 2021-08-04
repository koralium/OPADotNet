/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
