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
