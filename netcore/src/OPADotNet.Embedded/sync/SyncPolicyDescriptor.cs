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
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public class SyncPolicyDescriptor
    {
        internal SyncPolicyDescriptor(string policyName, string unknown = null)
        {
            PolicyName = policyName;
            Unknown = unknown;
        }

        public string PolicyName { get; }

        /// <summary>
        /// The unknown for this policy
        /// </summary>
        public string Unknown { get; }

        internal bool Found { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SyncPolicyDescriptor other)
            {
                return Equals(PolicyName, other.PolicyName) &&
                    Equals(Unknown, other.Unknown);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PolicyName, Unknown);
        }
    }
}
