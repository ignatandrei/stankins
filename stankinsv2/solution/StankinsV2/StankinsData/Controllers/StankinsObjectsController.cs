using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stankins.Interpreter;
using StankinsHelperCommands;

namespace StankinsDataWeb.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion( "1.0" )]
    [ApiController]
    public class StankinsObjectsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<ResultTypeStankins[]> Get()
        {
            return FindAssembliesToExecute.AddReferences(new FindAssembliesToExecute(null).FromType(typeof(RecipeFromFilePath)));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<ResultTypeStankins> Get(string id)
        {
            var ret=  FindAssembliesToExecute.AddReferences(new FindAssembliesToExecute(null).FromType(typeof(RecipeFromFilePath))).FirstOrDefault(it=>string.Equals(it.Name,id));
            if(ret == null)
            {
                return NotFound("could not find " + id);
            }
            return ret;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            //nothing yet
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
