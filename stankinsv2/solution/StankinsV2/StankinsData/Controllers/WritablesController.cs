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
            return new Writables(dir).Files();
        }

        // GET: api/Writables/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<string> Get([FromRoute] string id,[FromServices] IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");

            return await new Writables(dir).GetFileContents(id);
        }

        // POST: api/Writables
        [HttpPost]
        public async Task Post([FromBody] KeyValuePair<string,string> val, [FromServices]IHostingEnvironment hosting)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            await new Writables(dir).WriteFileContents(val.Key,val.Value);
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
            new Writables(dir).DeleteFile(id);
            Program.Shutdown();
        }
    }
}
