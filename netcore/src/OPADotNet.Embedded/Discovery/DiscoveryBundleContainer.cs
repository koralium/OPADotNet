using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryBundleContainer
    {
        public RestTarGzOptions RestTarGzOptions { get; set; }

        public SyncServiceHolder SyncServiceHolder { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DiscoveryBundleContainer other)
            {
                return Equals(RestTarGzOptions, other.RestTarGzOptions);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RestTarGzOptions);
        }
    }
}
