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
using OPADotNet.Core.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework.Controllers
{
    [Route("/v1/data")]
    [ApiController]
    internal class DataController : ControllerBase
    {
        private readonly TestOpaClientRest _testClient;
        public DataController(TestOpaClientRest testClient)
        {
            _testClient = testClient;
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> Get(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var data = await _testClient.GetData<object>(path);

            return Ok(new DataResponse<object>()
            {
                Result = data
            });
        }
    }
}
