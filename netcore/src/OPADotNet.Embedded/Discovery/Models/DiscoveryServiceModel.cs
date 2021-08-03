using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    class DiscoveryServiceModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("credentials")]
        public DiscoveryServiceCredentials Credentials { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }
    }
}
