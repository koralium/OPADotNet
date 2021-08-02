using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncOptions
    {
        public IReadOnlyList<SyncServiceHolder> SyncServices { get; }

        public SyncOptions(IReadOnlyList<SyncServiceHolder> syncServices)
        {
            SyncServices = syncServices;
        }
    }
}
