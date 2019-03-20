using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static StankinsCronFiles.CronExecution;

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
        public IEnumerable<CronExecutionFile> Get()
        {
            string dirPath = env.ContentRootPath;

            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            return Directory.GetFiles(dirPath)
                .Select(it => new CronExecutionFile(it))
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
        [HttpPost]
        public void Post([FromBody] CronExecutionFile value)
        {

            string dirPath = env.ContentRootPath;

            dirPath = Path.Combine(dirPath, "cronItems", "v1", value.Name);
            if (System.IO.File.Exists(dirPath))
            {
                throw new ArgumentException($"file {value.Name} already exists");
            }
            System.IO.File.WriteAllText(dirPath, value.WholeContent());


        }

        // PUT: api/CronExecutionFile/5
        [HttpPut]
        public void Put([FromBody] CronExecutionFile value)
        {
            CronExecutionFile data = Get(value.Name).Value;
            if (data == null)
            {
                return;
            }

            string dirPath = env.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1", value.Name);
            if (!System.IO.File.Exists(dirPath))
            {
                throw new FileNotFoundException($"file {value.Name} already exists");
            }
            System.IO.File.WriteAllText(dirPath, value.WholeContent());


        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            
            string dirPath = env.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            var writ=new Writables(dirPath);
            writ.DeleteFile(id);

        }
    }
}
