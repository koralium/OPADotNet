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
using OPADotNet.Core.Partial.Ast.Converters;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermConverter : JsonConverter<AstTerm>
    {
        private static PartialTermRefConverter refConverter = new PartialTermRefConverter();
        private static PartialTermVarConverter varConverter = new PartialTermVarConverter();
        private static PartialTermStringConverter stringConverter = new PartialTermStringConverter();
        private static PartialTermNumberConverter numberConverter = new PartialTermNumberConverter();
        private static PartialTermBooleanConverter booleanConverter = new PartialTermBooleanConverter();
        private static PartialTermObjectConverter objectConverter = new PartialTermObjectConverter();
        private static AstTermArrayConverter arrayConverter = new AstTermArrayConverter();
        private static PartialTermSetConverter setConverter = new PartialTermSetConverter();
        private static PartialTermNullConverter nullConverter = new PartialTermNullConverter();

        public override AstTerm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckType(JsonTokenType.StartObject);

            reader.ReadExpectPropertyName("type");
            string typeName = reader.ReadGetString();

            reader.ReadThrow();

            AstTerm returnValue;

            switch (typeName)
            {
                case "ref":
                    returnValue = refConverter.Read(ref reader, typeof(AstTermRef), options);
                    break;
                case "var":
                    returnValue = varConverter.Read(ref reader, typeof(AstTermVar), options);
                    break;
                case "string":
                    returnValue = stringConverter.Read(ref reader, typeof(AstTermString), options);
                    break;
                case "number":
                    returnValue = numberConverter.Read(ref reader, typeof(AstTermNumber), options);
                    break;
                case "boolean":
                    returnValue = booleanConverter.Read(ref reader, typeof(AstTermBoolean), options);
                    break;
                case "object":
                    returnValue = objectConverter.Read(ref reader, typeof(AstTermObject), options);
                    break;
                case "array":
                    returnValue = arrayConverter.Read(ref reader, typeof(AstTermArray), options);
                    break;
                case "set":
                    returnValue = setConverter.Read(ref reader, typeof(AstTermSet), options);
                    break;
                case "null":
                    returnValue = nullConverter.Read(ref reader, typeof(AstTermNull), options);
                    break;
                default:
                    throw new NotSupportedException($"term type '{typeName}' is not supported.");
            }

            reader.ReadCheckType(JsonTokenType.EndObject);

            return returnValue;
        }

        public override void Write(Utf8JsonWriter writer, AstTerm value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            string typeName = value.Type.ToString().ToLower();
            writer.WriteString("type", typeName);

            switch (typeName)
            {
                case "ref":
                    refConverter.Write(writer, value as AstTermRef, options);
                    break;
                case "var":
                    varConverter.Write(writer, value as AstTermVar, options);
                    break;
                case "string":
                    stringConverter.Write(writer, value as AstTermString, options);
                    break;
                case "number":
                    numberConverter.Write(writer, value as AstTermNumber, options);
                    break;
                case "boolean":
                    booleanConverter.Write(writer, value as AstTermBoolean, options);
                    break;
                case "object":
                    objectConverter.Write(writer, value as AstTermObject, options);
                    break;
                case "array":
                    arrayConverter.Write(writer, value as AstTermArray, options);
                    break;
                case "set":
                    setConverter.Write(writer, value as AstTermSet, options);
                    break;
                default:
                    throw new NotSupportedException($"term type '{typeName}' is not supported.");
            }

            writer.WriteEndObject();
        }
    }
}
