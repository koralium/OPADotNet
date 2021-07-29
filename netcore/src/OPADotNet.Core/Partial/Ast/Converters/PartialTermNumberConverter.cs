using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermNumberConverter : JsonConverter<AstTermNumber>
    {
        public override AstTermNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            var value = reader.ReadGetDecimal();

            return new AstTermNumber()
            {
                Value = value
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermNumber value, JsonSerializerOptions options)
        {
            writer.WriteNumber("value", value.Value);
        }
    }
}
