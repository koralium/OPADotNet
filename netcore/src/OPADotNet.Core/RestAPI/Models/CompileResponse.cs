using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.RestAPI.Models
{
    internal class CompileResponse
    {
        public AstQueries Result { get; set; }
    }
}
