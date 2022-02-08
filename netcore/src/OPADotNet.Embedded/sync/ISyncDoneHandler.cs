using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    public interface ISyncDoneHandler
    {
        Task SyncDone();
    }
}
