using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermArray : AstTerm
    {
        public override AstTermType Type => AstTermType.Array;

        public List<AstTerm> Value { get; set; }

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermArray(this);
        }
    }
}
