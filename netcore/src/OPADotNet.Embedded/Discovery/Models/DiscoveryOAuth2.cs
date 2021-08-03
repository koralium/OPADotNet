using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    internal class DiscoveryOAuth2
    {
        [JsonPropertyName("token_url")]
        public string TokenUrl { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("scopes")]
        public List<string> Scopes { get; set; }
    }
}
