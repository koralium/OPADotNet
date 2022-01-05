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
using OPADotNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.RestAPI
{
    internal class RestPreparedPartial : IPreparedPartial
    {
        private readonly RestOpaClient _restOpaClient;
        private readonly string _query;

        public RestPreparedPartial(RestOpaClient restOpaClient, string query)
        {
            _restOpaClient = restOpaClient;
            _query = query;
        }

        public void Dispose()
        {
            //Nothing to dispose
        }

        public Task<PartialResult> Partial(object input, IEnumerable<string> unknowns, bool explain = false)
        {
            return _restOpaClient.Compile(_query, input, unknowns.ToList());
        }
    }
}
