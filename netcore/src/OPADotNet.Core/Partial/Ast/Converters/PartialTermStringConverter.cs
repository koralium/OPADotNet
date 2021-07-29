using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermStringConverter : JsonConverter<AstTermString>
    {
        public override AstTermString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            var value = reader.ReadGetString();
            return new AstTermString()
            {
                Value = value
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermString value, JsonSerializerOptions options)
        {
            writer.WriteString("value", value.Value);
        }
    }
}
