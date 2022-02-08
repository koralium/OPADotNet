﻿using OPADotNet.Ast.Models;
using OPADotNet.Core.Extensions;
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

        public override bool Equals(object obj)
        {
            return obj is ExplanationRule body &&
                   Operation == body.Operation &&
                   QueryId == body.QueryId &&
                   ParentId == body.ParentId &&
                   Type == body.Type &&
                   Equals(Node, body.Node) &&
                   Message == body.Message &&
                   Locals.AreEqual(body.Locals) &&
                   Equals(Location, body.Location) &&
                   Equals(Node, body.Node);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private protected override AstNode GetNode() => Node;
    }
}
