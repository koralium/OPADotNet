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
