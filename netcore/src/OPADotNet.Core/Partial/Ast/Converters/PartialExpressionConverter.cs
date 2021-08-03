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
using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialExpressionConverter : JsonConverter<AstExpression>
    {
        public override AstExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckType(JsonTokenType.StartObject);

            reader.ReadExpectPropertyName("terms");

            reader.ReadThrow();

            List<AstTerm> terms = null;
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                terms = JsonSerializer.Deserialize<List<AstTerm>>(ref reader, options);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var term = JsonSerializer.Deserialize<AstTerm>(ref reader, options);
                terms = new List<AstTerm>() { term };
            }
            else
            {
                throw new JsonException("expected start array or start object for terms");
            }

            reader.ReadExpectPropertyName("index");
            reader.ReadCheckType(JsonTokenType.Number);
            int index = reader.GetInt32();

            reader.ReadUntil(JsonTokenType.EndObject);

            return new AstExpression()
            {
                Index = index,
                Terms = terms
            };
        }

        public override void Write(Utf8JsonWriter writer, AstExpression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("terms");

            if (value.Terms.Count == 1)
            {
                JsonSerializer.Serialize(writer, value.Terms.First(), options);
            }
            else
            {
                JsonSerializer.Serialize(writer, value.Terms, options);
            }

            writer.WriteNumber("index", value.Index);

            writer.WriteEndObject();
        }
    }
}
