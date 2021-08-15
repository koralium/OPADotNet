using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.RestAPI.Models
{
    internal class QueryResponse<TBinding>
    {
        [JsonPropertyName("result")]
        public List<TBinding> Result { get; set; }
    }
}
