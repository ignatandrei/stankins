using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stankins.Alive;

namespace StankinsStatusWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<ResultWithData[]>> Get(
            [FromServices]IOptionsSnapshot<MonitorOptions> opt)
            //[FromServices]MonitorOptions optVal)
        {
            var optVal = opt.Value;
            var dataPing= await Task.WhenAll(optVal.PingAddresses.Select(it => it.Execute()).ToArray());
            var webData= await Task.WhenAll(optVal.WebAdresses.Select(it => it.Execute()).ToArray());
            var all = dataPing.Union(webData)
                .Select(it => AliveStatus.FromTable(it))
                .SelectMany(it=>it)
                .Select(it=>optVal.DataFromResult(it))
                .ToArray();
            
            return all;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
