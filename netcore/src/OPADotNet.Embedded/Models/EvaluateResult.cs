using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Models
{
    internal class EvaluateResult<TBinding>
    {
        [JsonPropertyName("bindings")]
        public TBinding Bindings { get; set; }
    }
}
