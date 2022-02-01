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

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoverySyncContext : SyncContext
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly DiscoveryHandler _discoveryHandler;

        internal DiscoverySyncContext(
            OpaClientEmbedded opaClientEmbedded, 
            DiscoveryHandler discoveryHandler, 
            List<Type> syncDoneHandlers,
            IServiceProvider serviceProvider) 
            : base(new List<SyncPolicyDescriptor>(), opaClientEmbedded, syncDoneHandlers, serviceProvider)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _discoveryHandler = discoveryHandler;
        }

        public override SyncContextIterationPolicies NewIteration()
        {
            return new DiscoveryContextPolicies(_opaClientEmbedded, _discoveryHandler, this);
        }
    }
}
