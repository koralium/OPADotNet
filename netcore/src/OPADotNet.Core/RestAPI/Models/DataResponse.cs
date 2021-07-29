using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.RestAPI.Models
{
    internal class DataResponse<T>
    {
        [JsonPropertyName("result")]
        public T Result { get; set; }
    }
}
