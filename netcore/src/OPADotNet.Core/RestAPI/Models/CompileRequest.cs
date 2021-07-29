using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.RestAPI.Models
{
    internal class CompileRequest
    {
        public string Query { get; set; }

        public object Input { get; set; }

        public List<string> Unknowns { get; set; }
    }
}
