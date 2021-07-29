using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OPADotNet.AspNetCore.WebTest.Controllers
{
    //[Authorize(Policy = "read")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public class TestModel
        {
            public int clearance_level { get; set; }

            public int test_level { get; set; }

            public List<string> Members { get; set; }

            public bool Test { get; set; }
        }

        private readonly IAuthorizationService _authorizationService;

        public ValuesController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TestModel data)
        {
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "write");

            if (!authResult.Succeeded)
            {
                return Unauthorized();
            }

            return Ok();
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = new TestModel[] { 
                new TestModel(){
                    clearance_level = 20,
                    Members = new List<string>(),
                    test_level = 4,
                    Test = true
                },
                new TestModel(){
                    clearance_level = 20,
                    Members = new List<string>(),
                    test_level = 4,
                    Test = false
                }
            };

            await _authorizationService.AuthorizeAsync(HttpContext.User, data[0], "read");

            var (authResult, filteredData) = await _authorizationService.AuthorizeQueryable(HttpContext.User, data.AsQueryable(), "read");

            if (!authResult.Succeeded)
            {
                return Unauthorized();
            }

            return Ok(filteredData);
        }
    }
}
