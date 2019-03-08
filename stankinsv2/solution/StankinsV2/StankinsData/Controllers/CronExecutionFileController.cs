using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static StankinsDataWeb.classesToBeMoved.CronExecution;

namespace StankinsDataWeb.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CronExecutionFileController : ControllerBase
    {
        private readonly IHostingEnvironment env;

        public CronExecutionFileController(IHostingEnvironment env)
        {
            this.env = env;
        }
        // GET: api/CronExecutionFile
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string dirPath = env.ContentRootPath;

            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            return Directory.GetFiles(dirPath)
                .Select(it => Path.GetFileName(it))
                .ToArray();

        }

        // GET: api/CronExecutionFile/5
        [HttpGet("{id}")]
        public ActionResult<CronExecutionFile> Get(string id)
        {
            string dirPath = env.ContentRootPath;

            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            string f = Directory.GetFiles(dirPath)
                .FirstOrDefault(it => Path.GetFileName(it) == id);
            if (f == null)
            {
                return NotFound($"cannot find {id}");
            }
            return new CronExecutionFile(f);

        }

        // POST: api/CronExecutionFile
        [HttpPost("{id}")]
        public void Post([FromRoute]string id, [FromBody] string value)
        {
            string dirPath = env.ContentRootPath;

            dirPath = Path.Combine(dirPath, "cronItems", "v1", id);
            System.IO.File.WriteAllText(dirPath, value);


        }

        // PUT: api/CronExecutionFile/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] string value)
        {
            CronExecutionFile data = Get(id).Value;
            if (data == null)
            {
                return;
            }

            string dirPath = env.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1", id);
            System.IO.File.WriteAllText(dirPath, value);


        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            string dirPath = env.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1", id);
            System.IO.File.Delete(dirPath);

        }
    }
}
