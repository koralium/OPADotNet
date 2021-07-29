using OPADotNet.Ast.Models;
using OPADotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Partial.Ast.Converters
{
    internal class PartialBodyConverter : JsonConverter<AstBody>
    {
        public override AstBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckType(JsonTokenType.StartArray);

            var expressions = JsonSerializer.Deserialize<List<AstExpression>>(ref reader, options);
            return new AstBody()
            {
                Expressions = expressions
            };
        }

        public override void Write(Utf8JsonWriter writer, AstBody value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Expressions, options);
        }
    }
}
