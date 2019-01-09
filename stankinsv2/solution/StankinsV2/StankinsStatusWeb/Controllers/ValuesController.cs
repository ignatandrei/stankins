using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
            var toExec = optVal.ToExecuteCRON().Select(it => it.Execute()).ToArray();
            //var exec = await Task.WhenAll(optVal.AllItems().Select(it => it.Execute()).ToArray());
            var exec = await Task.WhenAll(toExec);
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
        public async Task<ActionResult<string>> Get(string id, [FromServices]IServiceScopeFactory sc)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),"MonitorData", id+".json");
            var text = await System.IO.File.ReadAllTextAsync(file);
            return Content(text);
        }
        [HttpPost("{id}")]
        public async Task Post(string id, [FromBody] MonitorOptions monitorOptions, [FromServices]IServiceScopeFactory sc)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "MonitorData", id + ".json");
            //TODO: seriazlie monitor options into file
            return ;

        }
        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] MonitorOptions monitorOptions, [FromServices]IServiceScopeFactory sc)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000 * 60);//one minute
            //TODO: solve this with a new RunTask instance that have also monitor options
            using (var rt = new RunTasks(sc,monitorOptions))
            {
                await rt.StartAsync(cts.Token);
            }
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
