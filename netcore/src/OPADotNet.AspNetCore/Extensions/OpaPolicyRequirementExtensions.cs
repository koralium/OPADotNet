using OPADotNet.AspNetCore.Requirements;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Extensions
{
    internal static class OpaPolicyRequirementExtensions
    {
        public static SyncPolicyDescriptor ToSyncPolicy(this OpaPolicyRequirement opaPolicyRequirement)
        {
            return new SyncPolicyDescriptor(opaPolicyRequirement.ModuleName, opaPolicyRequirement.DataName);
        }
    }
}
