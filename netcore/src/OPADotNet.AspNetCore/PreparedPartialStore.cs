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
using OPADotNet.AspNetCore.Requirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore
{
    /// <summary>
    /// Contains prepared partials
    /// </summary>
    internal class PreparedPartialStore
    {
        private readonly IOpaClient _opaClient;
        private readonly Dictionary<int, IPreparedPartial> _preparedPartials = new Dictionary<int, IPreparedPartial>();
        public PreparedPartialStore(IOpaClient opaClient)
        {
            _opaClient = opaClient;
        }

        public void PreparePartial(OpaPolicyRequirement opaPolicyRequirement)
        {
            _preparedPartials.Add(opaPolicyRequirement.Index, _opaClient.PreparePartial(opaPolicyRequirement.GetQuery()));
        }

        public IPreparedPartial GetPreparedPartial(OpaPolicyRequirement opaPolicyRequirement)
        {
            if (_preparedPartials.TryGetValue(opaPolicyRequirement.Index, out var preparedPartial))
            {
                return preparedPartial;
            }
            throw new InvalidOperationException("The opa policy requirements did not have any prepared partial");
        }
    }
}
