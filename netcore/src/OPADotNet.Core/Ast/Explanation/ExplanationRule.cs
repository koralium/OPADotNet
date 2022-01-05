using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public class ExplanationRule : ExplanationNode
    {
        public override ExplanationType Type => ExplanationType.Rule;

        [JsonPropertyName("node")]
        public new AstPolicyRule Node { get; set; }

        private protected override AstNode GetNode() => Node;
    }
}
