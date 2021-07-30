using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExample.Controllers
{
    public class MockLoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
