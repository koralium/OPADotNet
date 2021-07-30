using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Sync
{
    public class OpaServerSyncOptions
    {
        public string OpaServerUrl { get; set; }

        public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    }
}
