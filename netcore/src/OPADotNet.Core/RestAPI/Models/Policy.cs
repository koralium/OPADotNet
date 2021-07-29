using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Ast.Models
{
    public class Policy
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("raw")]
        public string Raw { get; set; }

        [JsonPropertyName("ast")]
        public AstPolicy Ast { get; set; }
    }
}
