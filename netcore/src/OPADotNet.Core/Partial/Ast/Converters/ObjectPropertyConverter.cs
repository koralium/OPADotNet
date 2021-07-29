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
