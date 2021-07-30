using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public interface ISyncContextPolicies
    {
        Policy CompilePolicy(string fileName, string rawText);

        void AddPolicy(Policy policy);

        void AddPolicies(IEnumerable<Policy> policies);
    }
}
