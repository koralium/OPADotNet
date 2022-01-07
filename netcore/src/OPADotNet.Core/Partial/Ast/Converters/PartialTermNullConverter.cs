using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Partial.Ast.Converters
{
    internal class PartialTermNullConverter : JsonConverter<AstTermNull>
    {
        public override AstTermNull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            reader.ReadCheckType(JsonTokenType.StartObject);
            reader.ReadCheckType(JsonTokenType.EndObject);
            return new AstTermNull();
        }

        public override void Write(Utf8JsonWriter writer, AstTermNull value, JsonSerializerOptions options)
        {
            writer.WriteStartObject("value");
            writer.WriteEndObject();
        }
    }
}
