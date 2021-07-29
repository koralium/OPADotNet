using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast.Models
{
    internal class Queries : Node
    {
        public List<Query> OrQueries { get; set; }

        public override T Accept<T>(ExpressionAstVisitor<T> visitor)
        {
            return visitor.VisitQueries(this);
        }
    }
}
