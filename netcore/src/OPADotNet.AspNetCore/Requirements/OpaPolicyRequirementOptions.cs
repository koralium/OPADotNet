using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Requirements
{
    public class OpaPolicyRequirementOptions
    {
        public string InputResourceName { get; set; } = "resource";

        public string CustomQuery { get; set; }

        public string CustomUnknown { get; set; }
    }
}
