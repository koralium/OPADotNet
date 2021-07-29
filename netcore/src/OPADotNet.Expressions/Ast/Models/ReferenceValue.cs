using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal abstract class ReferenceValue : Node
    {
        public abstract ReferenceType Type { get; }
        public string Value { get; set; }
    }
}
