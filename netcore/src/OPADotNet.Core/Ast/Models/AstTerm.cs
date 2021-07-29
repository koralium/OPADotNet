using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public abstract class AstTerm : AstNode
    {
        public abstract AstTermType Type { get; }
    }
}
