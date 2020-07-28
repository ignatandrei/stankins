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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class @(dt.TableName)Controller : ControllerBase
    {
        private readonly IRepository<@(dt.TableName)> _repository;

        public @(dt.TableName)Controller(IRepository<@(dt.TableName)> repository)
        {
            _repository = repository;
        }

        // GET: api/@(dt.TableName)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<@(dt.TableName)>>> GetAll()
        {
            return await _repository.GetAll();
        }

        // GET: api/@(dt.TableName)/5
        [HttpGet("{id}")]
        public async Task<ActionResult<@(dt.TableName)>> Get(long id)
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
        public async Task<IActionResult> Put(long id, @(dt.TableName) record)
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
        public async Task<ActionResult<@(dt.TableName)>> Insert(@(dt.TableName) record)
        {
            await _repository.Insert(record);

            return CreatedAtAction("Get@(dt.TableName)", new { id = record.ID }, record);
        }

        // DELETE: api/@(dt.TableName)/5
        [HttpDelete("{id}")]
        public async Task<long> Delete(long id)
        {
            
            await _repository.Delete( new @(dt.TableName)(){
                ID=id
            });


            return id;
        }

       
    }
}
