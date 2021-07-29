using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncPolicy
    {
        public string PolicyName { get; set; }

        public HashSet<string> DataSets { get; set; }

        public string Raw { get; set; }
    }
}
