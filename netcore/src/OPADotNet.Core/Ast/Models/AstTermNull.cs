using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public class AstTermNull : AstTerm
    {
        public override AstTermType Type => AstTermType.Null;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermNull(this);
        }
    }
}
