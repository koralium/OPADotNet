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
using Microsoft.AspNetCore.Mvc;
using OPADotNet.Core.Models;
using OPADotNet.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework.Controllers
{
    [Route("/v1/compile")]
    [ApiController]
    internal class CompileController : ControllerBase
    {
        private readonly TestOpaClientRest _testClient;
        public CompileController(TestOpaClientRest testClient)
        {
            _testClient = testClient;
        }

        [HttpPost]
        public async Task<PartialResult> Post([FromBody] CompileRequest compileRequest)
        {
            var prepared = _testClient.PreparePartial(compileRequest.Query);
            var queries = await prepared.Partial(compileRequest.Input, compileRequest.Unknowns);

            return queries;
        }
    }
}
