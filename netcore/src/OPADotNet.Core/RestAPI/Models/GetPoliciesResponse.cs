using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.RestAPI.Models
{
    public class GetPoliciesResponse
    {
        [JsonPropertyName("result")]
        public List<Policy> Result { get; set; }
    }
}
