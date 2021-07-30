using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    internal class LocalSync : SyncServiceBase
    {
        private readonly LocalSyncOptions _localSyncOptions;
        public LocalSync(LocalSyncOptions localSyncOptions)
        {
            _localSyncOptions = localSyncOptions;
        }

        public override Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken)
        {
            foreach (var localData in _localSyncOptions.LocalDatas)
            {
                syncContextData.AddData(localData.Path, localData.Content);
            }
            return Task.CompletedTask;
        }

        public override Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken)
        {
            foreach (var policy in _localSyncOptions.Policies)
            {
                var compiledPolicy = syncContextPolicies.CompilePolicy(Guid.NewGuid().ToString(), policy);
                syncContextPolicies.AddPolicy(compiledPolicy);
            }
            return Task.CompletedTask;
        }
    }
}
