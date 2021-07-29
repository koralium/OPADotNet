using Microsoft.AspNetCore.Mvc;
using OPADotNet.Ast;
using OPADotNet.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework.Controllers
{
    [Route("/v1/policies")]
    [ApiController]
    internal class PoliciesController : ControllerBase
    {
        private readonly TestOpaClientRest _testClient;
        public PoliciesController(TestOpaClientRest testClient)
        {
            _testClient = testClient;
        }

        [HttpGet]
        public async Task<GetPoliciesResponse> Get()
        {
            var policies = await _testClient.GetPolicies();
            var resp = new GetPoliciesResponse()
            {
                Result = policies
            };

            return resp;
        }
    }
}
