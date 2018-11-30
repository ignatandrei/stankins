using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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
            [FromServices]IMediator mediator,
            [FromServices]IOptionsSnapshot<MonitorOptions> opt)
            //[FromServices]MonitorOptions optVal)
        {
            var optVal = opt.Value;
            var exec = await Task.WhenAll(optVal.AllItems().Select(it => it.Execute()).ToArray());
            var all = exec
                .Select(it => AliveStatus.FromTable(it))
                .SelectMany(it=>it)
                .Select(it=>optVal.DataFromResult(it))
                .ToArray();
            foreach(var item in all)
            {
                await mediator.Publish(item);
            }
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
