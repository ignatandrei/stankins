@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
    string repo= @dt.TableName  + "_Repository";

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWEBAPI_DAL;
using TestWebAPI_BL;
using TestWEBAPI_DAL;

namespace TestWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class @(dt.TableName)RESTController : ControllerBase
    {
        private readonly IRepository<@(dt.TableName)> _repository;

        public @(dt.TableName)RESTController(IRepository<@(dt.TableName)> repository)
        {
            _repository = repository;
        }

        // GET: api/@(dt.TableName)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<@(dt.TableName)>>> Get@(dt.TableName)()
        {
            return await _repository.GetAll();
        }

        // GET: api/@(dt.TableName)/5
        [HttpGet("{id}")]
        public async Task<ActionResult<@(dt.TableName)>> Get@(dt.TableName)(long id)
        {
            var record = await _repository.FindAfterId(id);

            if (record == null)
            {
                return NotFound($"cannot find record with id = {id}");
            }

            return record;
        }

        // PUT: api/@(dt.TableName)/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put@(dt.TableName)(long id, @(dt.TableName) record)
        {
            if (id != record.ID)
            {
                return BadRequest();
            }
            
             _repository.Update(record);
            
            return NoContent();
        }

        // POST: api/@(dt.TableName)
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<@(dt.TableName)>> Post@(dt.TableName)(@(dt.TableName) record)
        {
            await _repository.Update(record);

            return CreatedAtAction("Get@(dt.TableName)", new { id = record.ID }, record);
        }

        // DELETE: api/@(dt.TableName)/5
        [HttpDelete("{id}")]
        public async Task<long> Delete@(dt.TableName)(long id)
        {
            
            await _repository.Delete( new @(dt.TableName)(){
                ID=id
            });


            return id;
        }

       
    }
}
