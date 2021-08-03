using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    internal class DiscoveryServiceCredentials
    {
        [JsonPropertyName("oauth2")]
        public DiscoveryOAuth2 OAuth2 { get; set; }
    }
}
