using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StankinsDataWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WritablesController : ControllerBase
    {
        // GET: api/Writables
        [HttpGet]
        public IEnumerable<KeyValuePair<string,string>> Get([FromServices]IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            return Directory.GetFiles(dir)
                .Select(it=>new KeyValuePair<string,string>(Path.GetFileName(it),System.IO.File.ReadAllText(it)))
                .ToArray();
        }

        // GET: api/Writables/5
        [HttpGet("{id}", Name = "Get")]
        public string Get([FromRoute] string id,[FromServices] IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");

            return System.IO.File.ReadAllText(Path.Combine(dir,id));
        }

        // POST: api/Writables
        [HttpPost]
        public void Post([FromBody] KeyValuePair<string,string> val, [FromServices]IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            System.IO.File.WriteAllText(Path.Combine(dir,val.Key),val.Value);
            Program.Shutdown();
        }

        //// PUT: api/Writables/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id,[FromServices]IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            System.IO.File.Delete(Path.Combine(dir,id));
            Program.Shutdown();
        }
    }
}
