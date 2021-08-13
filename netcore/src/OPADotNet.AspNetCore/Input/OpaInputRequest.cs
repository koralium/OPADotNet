using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Input
{
    internal class OpaInputRequest
    {
        [JsonPropertyName("routeValues")]
        public IDictionary<string, object> RouteValues { get; set; }

        [JsonPropertyName("path")]
        public List<string> Path { get; set; }

        [JsonPropertyName("query")]
        public Dictionary<string, List<string>> Query { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }
    }
}
