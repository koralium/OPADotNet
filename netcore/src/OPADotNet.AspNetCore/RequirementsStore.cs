/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
