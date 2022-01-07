using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public class ExplanationBinding
    {
        [JsonPropertyName("key")]
        public AstTerm Key { get; set; }

        [JsonPropertyName("value")]
        public AstTerm Value { get; set; }
    }
}
