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
using OPADotNet.Embedded;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework
{
    internal class TestOpaClientRest : RestOpaClient
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<Policy> _policies;
        public TestOpaClientRest(OpaClientEmbedded opaClientEmbedded, List<Policy> policies) : base("http://localhost")
        {
            _opaClientEmbedded = opaClientEmbedded;
            _policies = policies;
        }

        public override Task<List<Policy>> GetPolicies()
        {
            return Task.FromResult(_policies.ToList());
        }

        public override IPreparedPartial PreparePartial(string query)
        {
            return _opaClientEmbedded.PreparePartial(query);
        }

        public override Task<T> GetData<T>(string path)
        {
            var txn = _opaClientEmbedded.OpaStore.NewTransaction(false);
            var data = txn.Read<T>(path);
            txn.Commit();
            return Task.FromResult(data);
        }

        public override Task<string> GetDataJson(string path)
        {
            var txn = _opaClientEmbedded.OpaStore.NewTransaction(false);
            var data = txn.Read(path);
            txn.Commit();
            return Task.FromResult(data);
        }
    }
}
