using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialTermVarConverter : JsonConverter<AstTermVar>
    {
        public override AstTermVar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ExpectPropertyName("value");
            var value = reader.ReadGetString();
            return new AstTermVar()
            {
                Value = value
            };
        }

        public override void Write(Utf8JsonWriter writer, AstTermVar value, JsonSerializerOptions options)
        {
            writer.WriteString("value", value.Value);
        }
    }
}
