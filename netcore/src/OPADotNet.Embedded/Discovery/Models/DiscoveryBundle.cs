using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    class DiscoveryBundle
    {
        [JsonPropertyName("service")]
        public string Service { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("polling")]
        public DiscoveryBundlePolling Polling { get; set; }
    }
}
