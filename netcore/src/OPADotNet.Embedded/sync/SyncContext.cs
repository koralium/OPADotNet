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
using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    /// <summary>
    /// Service that helps with synchronisation
    /// </summary>
    public class SyncContext
    {
        private readonly List<SyncPolicyDescriptor> _modules;
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly IReadOnlyList<Type> _syncDoneHandlers;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, SyncPolicy> _existingSyncPolicies = new Dictionary<string, SyncPolicy>();

        internal SyncContext(
            List<SyncPolicyDescriptor> modules, 
            OpaClientEmbedded opaClientEmbedded, 
            IReadOnlyList<Type> syncDoneHandlers, 
            IServiceProvider serviceProvider)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _modules = modules;
            _syncDoneHandlers = syncDoneHandlers;
            _serviceProvider = serviceProvider;
        }

        public virtual SyncContextIterationPolicies NewIteration()
        {
            return new SyncContextIterationPolicies(_opaClientEmbedded, _modules, _existingSyncPolicies, this);
        }

        internal async Task Done()
        {
            // Call listeners
            if (_serviceProvider != null)
            {
                using var scope = _serviceProvider.CreateScope();
                foreach (var handler in _syncDoneHandlers)
                {
                    var syncDoneHandler = scope.ServiceProvider.GetService(handler) as ISyncDoneHandler;
                    if (syncDoneHandler != null)
                    {
                        await syncDoneHandler.SyncDone();
                    }
                }
            }
        }
    }
}
