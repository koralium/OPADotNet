using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Internal
{
    internal static class JsonOptionsHelper
    {
        internal static JsonSerializerOptions SerializerOptions { get; } = GetSerializerOptions();

        private static JsonSerializerOptions GetSerializerOptions()
        {
            var opt = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            opt.Converters.Add(new JsonStringEnumConverter());
            return opt;
        }
    }
}
