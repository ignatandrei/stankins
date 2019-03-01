using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsDataWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WritablesController : ControllerBase
    {
        // GET: api/Writables
        [HttpGet]
        public IEnumerable<KeyValuePair<string, string>> Get([FromServices]IHostingEnvironment hosting)
        {
           
            string dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            return new Writables(dir).Files();
        }

        // GET: api/Writables/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<string> Get([FromRoute] string id, [FromServices] IHostingEnvironment hosting)
        {
            string dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");

            return await new Writables(dir).GetFileContents(id);
        }

        // POST: api/Writables
        [HttpPost]
        public async Task Post([FromBody] KeyValuePair<string, string> val, [FromServices]IHostingEnvironment hosting)
        {
            string dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            await new Writables(dir).WriteFileContents(val.Key, val.Value);
            Program.Shutdown();
        }

        //// PUT: api/Writables/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id, [FromServices]IHostingEnvironment hosting)
        {

            string dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            if(id != null){
                new Writables(dir).DeleteFile(id);
            }
            else
            {
                var w=new Writables(dir);
                var files=this.Get(hosting);
                files.ToList().ForEach(it=> w.DeleteFile(id));
            }
            Program.Shutdown();
        }

        
    }
}
