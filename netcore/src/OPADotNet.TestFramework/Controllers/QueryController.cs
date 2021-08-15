using Microsoft.AspNetCore.Mvc;
using OPADotNet.Core.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework.Controllers
{
    [Route("/v1/query")]
    [ApiController]
    internal class QueryController : ControllerBase
    {
        private static object emptyInput = new object();
        private readonly TestOpaClientRest _testClient;

        public QueryController(TestOpaClientRest testClient)
        {
            _testClient = testClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string q)
        {
            var preparedEval = _testClient.PrepareEvaluation(q);
            var response = await preparedEval.Evaluate<JsonElement>();
            return Ok(new DataResponse<object>()
            {
                Result = response
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QueryRequest queryRequest)
        {
            var preparedEval = _testClient.PrepareEvaluation(queryRequest.Query);
            var response = await preparedEval.Evaluate<JsonElement>(queryRequest.Input);
            return Ok(new DataResponse<object>()
            {
                Result = response
            });
        }
    }
}
