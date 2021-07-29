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
