using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Cryptography;
namespace Hashr.Controllers
{
    public class ValuesController : Controller
    {        // POST api/values
        [HttpPost]
        [Route("hashme")]
        public async Task<string> HashMe()
        {
            using(var memstream = new MemoryStream())
            using(var sha = SHA256.Create())
            {
                await this.Request.Body.CopyToAsync(memstream);
                return System.Text.Encoding.ASCII.GetString(sha.ComputeHash(memstream.ToArray()));
            }
        }
    }
}
