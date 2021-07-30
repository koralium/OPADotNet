using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class AstTermArrayConverter : JsonConverter<AstTermArray>
    {
        public override AstTermArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            reader.ReadCheckType(JsonTokenType.StartArray);

            var values = JsonSerializer.Deserialize<List<AstTerm>>(ref reader, options);

            return new AstTermArray()
            {
                Value = values
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermArray value, JsonSerializerOptions options)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
