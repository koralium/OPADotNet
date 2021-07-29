using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermBooleanConverter : JsonConverter<AstTermBoolean>
    {
        public override AstTermBoolean Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            var value = reader.ReadGetBoolean();

            return new AstTermBoolean()
            {
                Value = value
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermBoolean value, JsonSerializerOptions options)
        {
            writer.WriteBoolean("value", value.Value);
        }
    }
}
