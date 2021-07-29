using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public abstract class AstNode
    {
        public abstract T Accept<T>(AstVisitor<T> visitor);
    }
}
