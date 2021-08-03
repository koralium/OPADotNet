using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    class DiscoveryBindingResult
    {
        [JsonPropertyName("bundles")]
        public Dictionary<string, DiscoveryBundle> Bundles { get; set; }

        [JsonPropertyName("services")]
        public Dictionary<string, DiscoveryServiceModel> Services { get; set; }
    }
}
