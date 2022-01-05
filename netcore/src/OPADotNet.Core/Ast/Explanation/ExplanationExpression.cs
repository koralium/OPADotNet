using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    public class ExplanationExpression : ExplanationNode
    {
        public override ExplanationType Type => ExplanationType.Expression;

        [JsonPropertyName("node")]
        public new AstExpression Node { get; set; }

        private protected override AstNode GetNode() => Node;
    }
}
