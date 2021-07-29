using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    public class OpaBuilderPolicy
    {
        public string PolicyName { get; }

        public string Module { get; }

        public OpaBuilderPolicy(string policyName, string module)
        {
            PolicyName = policyName;
            Module = module;
        }
    }
}
