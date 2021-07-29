using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal abstract class Node
    {
        public abstract T Accept<T>(ExpressionAstVisitor<T> visitor);
    }
}
