using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    internal class DiscoveryBundlePolling
    {
        [JsonPropertyName("min_delay_seconds")]
        public long? MinDelaySeconds { get; set; }

        [JsonPropertyName("max_delay_seconds")]
        public long? MaxDelaySeconds { get; set; }
    }
}
