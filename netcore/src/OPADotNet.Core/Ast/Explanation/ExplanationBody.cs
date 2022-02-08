using OPADotNet.Ast.Models;
using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public class ExplanationBody : ExplanationNode
    {
        public override ExplanationType Type => ExplanationType.Body;

        [JsonPropertyName("node")]
        public new AstBody Node { get; set; }

        private protected override AstNode GetNode() => Node;

        
    }
}
