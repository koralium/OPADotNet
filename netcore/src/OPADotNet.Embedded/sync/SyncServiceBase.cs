using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    public abstract class SyncServiceBase : ISyncService
    {
        public abstract Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken);

        public async Task FullLoad(SyncContext syncContext, CancellationToken cancellationToken)
        {
            var policyStep = syncContext.NewIteration();
            await LoadPolices(policyStep, cancellationToken);
            var dataStep = policyStep.Next();
            await LoadData(dataStep, cancellationToken);
            dataStep.Done();
        }

        public abstract Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken);

        public abstract Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken);
    }
}
