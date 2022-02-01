using OPADotNet.Ast.Models;
using OPADotNet.Core.Ast.Explanation;
using OPADotNet.Extensions;
using OPADotNet.Partial.Ast.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Partial.Ast.Converters
{
    internal class ExplanationConverter : JsonConverter<ExplanationNode>
    {
        private static readonly PartialBodyConverter bodyReader = new PartialBodyConverter();
        private static readonly PartialExpressionConverter partialExpressionConverter = new PartialExpressionConverter();

        public override ExplanationNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckType(JsonTokenType.StartObject);

            reader.ReadExpectPropertyName("op");
            var op = reader.ReadGetString();
            reader.ReadExpectPropertyName("query_id");
            var queryId = reader.ReadGetInt();
            reader.ReadExpectPropertyName("parent_id");
            var parentId = reader.ReadGetInt();
            reader.ReadExpectPropertyName("type");
            var type = reader.ReadGetString();

            reader.ReadExpectPropertyName("node");
            ExplanationNode node;
            reader.Read();
            switch (type)
            {
                case "body":
                    var body = bodyReader.Read(ref reader, typeof(AstBody), options);
                    node = new ExplanationBody()
                    {
                        Node = body,
                        Operation = op,
                        ParentId = parentId,
                        QueryId = queryId,
                    };
                    break;
                case "expr":
                    var expr = partialExpressionConverter.Read(ref reader, typeof(AstExpression), options);

                    node = new ExplanationExpression()
                    {
                        Node = expr,
                        Operation = op,
                        ParentId = parentId,
                        QueryId = queryId,
                    };
                    break;
                case "rule":
                    var rule = JsonSerializer.Deserialize<AstPolicyRule>(ref reader, options);

                    node = new ExplanationRule()
                    {
                        Node = rule,
                        Operation = op,
                        ParentId = parentId,
                        QueryId = queryId,
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            reader.ReadExpectPropertyName("locals");
            node.Locals = JsonSerializer.Deserialize<List<ExplanationBinding>>(ref reader, options);

            reader.ReadExpectPropertyName("location");
            node.Location = JsonSerializer.Deserialize<ExplanationLocation>(ref reader, options);

            reader.Read();
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                reader.ExpectPropertyName("message");
                node.Message = reader.ReadGetString();
            }
            else if (reader.TokenType == JsonTokenType.EndObject)
            {
                return node;
            }

            reader.ReadUntil(JsonTokenType.EndObject);

            return node;
        }

        public override void Write(Utf8JsonWriter writer, ExplanationNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("op", value.Operation);
            writer.WriteNumber("query_id", value.QueryId);
            writer.WriteNumber("parent_id", value.ParentId);

            switch (value.Type)
            {
                case ExplanationType.Body:
                    writer.WriteString("type", "body");
                    writer.WritePropertyName("node");
                    bodyReader.Write(writer, value.Node as AstBody, options);
                    break;
                case ExplanationType.Expression:
                    writer.WriteString("type", "expr");
                    writer.WritePropertyName("node");
                    partialExpressionConverter.Write(writer, value.Node as AstExpression, options);
                    break;
                case ExplanationType.Rule:
                    writer.WriteString("type", "rule");
                    writer.WritePropertyName("node");
                    JsonSerializer.Serialize(writer, value.Node as AstPolicyRule, options);
                    break;
                default:
                    throw new NotImplementedException();
            }

            writer.WriteEndObject();
        }
    }
}
