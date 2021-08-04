/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    internal class LocalSync : SyncServiceBase<LocalSync>
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
