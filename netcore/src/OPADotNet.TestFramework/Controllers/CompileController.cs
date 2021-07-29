using Microsoft.AspNetCore.Mvc;
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
        public async Task<CompileResponse> Post([FromBody] CompileRequest compileRequest)
        {
            var prepared = _testClient.PreparePartial(compileRequest.Query);
            var queries = await prepared.Partial(compileRequest.Input, compileRequest.Unknowns);

            return new CompileResponse()
            {
                Result = queries
            };
        }
    }
}
