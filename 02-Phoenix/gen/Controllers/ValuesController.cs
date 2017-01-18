using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
namespace gen.Controllers
{
    
    public class ValuesController : Controller
    {
        static System.Security.Cryptography.RandomNumberGenerator gen = RandomNumberGenerator.Create();
        // GET api/values
        [HttpGet]
        [RouteAttribute("")]
        public string Get() => "gen running";

        [HttpGet]
        [RouteAttribute("/{size:int}")]
        public IActionResult Gen(int size)
        {
            System.Threading.Thread.Sleep(400); //emulate bad code
            var bits = new byte[size];
            gen.GetBytes(bits);
            return File(bits, "application/octet-stream");
        }
    }
}
