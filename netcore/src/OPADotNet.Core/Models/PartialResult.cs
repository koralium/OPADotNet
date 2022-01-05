using OPADotNet.Ast.Models;
using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Core.Models
{
    public class PartialResult
    {
        public AstQueries Result { get; set; }

        public List<ExplanationNode> Explanation { get; set; }
    }
}
