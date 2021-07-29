using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermObjectConverter : JsonConverter<AstTermObject>
    {
        public override AstTermObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");

            reader.ReadCheckType(JsonTokenType.StartArray);

            var properties = JsonSerializer.Deserialize<List<AstObjectProperty>>(ref reader, options);

            return new AstTermObject()
            {
                Value = properties
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermObject value, JsonSerializerOptions options)
        {
            writer.WritePropertyName("value");

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
