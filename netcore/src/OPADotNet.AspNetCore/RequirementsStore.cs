using OPADotNet.AspNetCore.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore
{
    /// <summary>
    /// A store that keeps track of all the used requirements
    /// </summary>
    internal static class RequirementsStore
    {
        private static object lockObject = new object();
        private static int index;
        private static List<WeakReference<OpaPolicyRequirement>> requirements = new List<WeakReference<OpaPolicyRequirement>>();
        public static void AddRequirement(OpaPolicyRequirement opaPolicyRequirement)
        {
            lock(lockObject) {
                index++;
                //Set the index of the policy
                opaPolicyRequirement.Index = index;
                requirements.Add(new WeakReference<OpaPolicyRequirement>(opaPolicyRequirement));
            }
            
        }

        public static IEnumerable<OpaPolicyRequirement> GetRequirements()
        {
            lock (lockObject)
            {
                return requirements.Select(x =>
                {
                    if (x.TryGetTarget(out var policy))
                    {
                        return policy;
                    }
                    return null;
                }).Where(x => x != null);
            }
        }
    }
}
