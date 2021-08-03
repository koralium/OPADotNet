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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class ObjectPropertyConverter : JsonConverter<AstObjectProperty>
    {
        public override AstObjectProperty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckType(JsonTokenType.StartArray);

            var list = JsonSerializer.Deserialize<List<AstTerm>>(ref reader, options);

            return new AstObjectProperty()
            {
                Values = list
            };
        }

        public override void Write(Utf8JsonWriter writer, AstObjectProperty value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Values, options);
        }
    }
}
