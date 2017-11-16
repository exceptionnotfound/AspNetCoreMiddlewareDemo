using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMiddlewareDemo.Controllers
{
    public class HomeController : Controller
    {
        [AcceptVerbs("GET", "POST")]
        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult Error()
        {
            return NotFound();
        }
    }
}
