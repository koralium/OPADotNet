using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncPolicyDescriptor
    {
        public string PolicyName { get; set; }

        /// <summary>
        /// The unknown for this policy
        /// </summary>
        public string Unknown { get; set; }

        internal bool Found { get; set; }
    }
}
