using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    class DiscoveryModel
    {
        [JsonPropertyName("result")]
        public DiscoveryBindingResult Result { get; set; }
    }
}
