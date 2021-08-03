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
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Discovery
{
    internal class DiscoveryContextData : SyncContextIterationData
    {
        private readonly DiscoveryHandler _discoveryHandler;

        internal DiscoveryContextData(OpaClientEmbedded opaClientEmbedded, List<SyncPolicy> policies, DiscoveryHandler discoveryHandler) 
            : base(opaClientEmbedded, policies, new DataSetNode("data", false)
                {
                    UsedInPolicyMutable = true
                })
        {
            _discoveryHandler = discoveryHandler;
        }

        public override async Task Done()
        {
            await base.Done();

            //Update the discovery handler
            await _discoveryHandler.Update();
        }
    }
}
