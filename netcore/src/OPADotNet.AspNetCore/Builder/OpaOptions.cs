using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class OpaOptions
    {
        public Uri OpaServer { get; }

        public bool UseEmbedded { get; }

        public IReadOnlyList<ISyncService> SyncServices { get; }

        public IReadOnlyList<Type> SyncServiceTypes { get; }

        public OpaOptions(
            Uri opaServer,
            bool useEmbedded, 
            IReadOnlyList<ISyncService> syncServices,
            IReadOnlyList<Type> syncServiceTypes)
        {
            OpaServer = opaServer;
            UseEmbedded = useEmbedded;
            SyncServices = syncServices;
            SyncServiceTypes = syncServiceTypes;
        }
    }
}
