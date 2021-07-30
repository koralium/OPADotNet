using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    public interface ISyncService
    {
        /// <summary>
        /// This function must load in all the data from the sync source, the task should return when the data is loaded.
        /// </summary>
        /// <param name="syncContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FullLoad(SyncContext syncContext, CancellationToken cancellationToken);

        Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken);

        Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken);

        Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken);
    }
}
