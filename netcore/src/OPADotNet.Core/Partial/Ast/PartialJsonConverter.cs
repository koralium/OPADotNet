﻿using OPADotNet.Ast.Models;
using OPADotNet.Partial.Ast.Converters;
using OPADotNet.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast
{
    /// <summary>
    /// Helper class that converts partial json result into its AST models
    /// </summary>
    public static class PartialJsonConverter
    {
        private static readonly JsonSerializerOptions _serializerOptions = CreateJsonSerializer();

        private static JsonSerializerOptions CreateJsonSerializer()
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            AddJsonConverters(serializerOptions.Converters);

            return serializerOptions;
        }

        internal static void AddJsonConverters(IList<JsonConverter> converters)
        {
            converters.Add(new PartialTermConverter());
            converters.Add(new PartialBodyConverter());
            converters.Add(new ObjectPropertyConverter());
            converters.Add(new PartialExpressionConverter());
        }

        public static string SerializePolicies(GetPoliciesResponse getPoliciesResponse)
        {
            return JsonSerializer.Serialize(getPoliciesResponse, _serializerOptions);
        }

        public static AstQueries ReadPartialQuery(string json)
        {
            return JsonSerializer.Deserialize<AstQueries>(json, _serializerOptions);
        }

        public static GetPoliciesResponse ReadPolicyResponse(string json)
        {
            return JsonSerializer.Deserialize<GetPoliciesResponse>(json, _serializerOptions);
        }

        internal static CompileResponse ReadCompileResponse(string json)
        {
            return JsonSerializer.Deserialize<CompileResponse>(json, _serializerOptions);
        }

        internal static Dictionary<string, AstPolicy> ReadEmbeddedPolicies(string json)
        {
            return JsonSerializer.Deserialize<Dictionary<string, AstPolicy>>(json, _serializerOptions);
        }

        internal static AstPolicy ReadEmbeddedPolicy(string json)
        {
            return JsonSerializer.Deserialize<AstPolicy>(json, _serializerOptions);
        }
    }
}
