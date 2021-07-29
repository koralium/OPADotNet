using OPADotNet.Ast.Models;
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
                default:
                    throw new NotSupportedException($"term type '{typeName}' is not supported.");
            }

            writer.WriteEndObject();
        }
    }
}
