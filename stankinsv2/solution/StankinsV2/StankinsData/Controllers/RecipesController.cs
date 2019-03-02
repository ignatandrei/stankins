using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Stankins.Interfaces;
using Stankins.Interpreter;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsDataWeb.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;

        public RecipesController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpGet]
        public ActionResult<Recipe[]> Get()
        {
            return RecipeFromString.RecipesFromBaseObjects().Union(RecipeFromString.RecipesFromFolder()).ToArray();
        }
        [HttpGet("{id}/{table?}")]
        public ActionResult<string[]> Get(string id, string table)
        {
            if (!memoryCache.TryGetValue(id, out IDataToSent data))
            {
                return NotFound($"id not found {id}");
            }
            System.Collections.Generic.IList<ITable> tables = data.Metadata.Tables;
            if (string.IsNullOrWhiteSpace(table))
            {
                return tables.Select(it => it.Name).ToArray();
            }
            ITable t = tables.FirstOrDefault(it => string.Equals(it.Name, table, StringComparison.InvariantCultureIgnoreCase));
            if (t == null)
            {
                return NotFound($"table {table} not found");
            }
            DataTable ret = data.DataToBeSentFurther[t.Id];
            string[] obj = JArray.FromObject(ret).Select(it => it.ToString(Formatting.None)).ToArray();
            return obj;


        }
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] Recipe recipe, [FromServices] IAsyncPolicy policy)
        {
            string cnt = recipe.Content;
            if (memoryCache.TryGetValue<string>(cnt, out string id))
            {
                return id;
            }
            var data = await policy.ExecuteAsync(() => new RecipeFromString(cnt).TransformData(null));
            
            if (data == null)
            {
                return BadRequest($"cannot process {recipe.Content}");
            }
            await memoryCache.GetOrCreateAsync<string>(cnt, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(1);
                        return Task.FromResult(data.id);
                    });

            await memoryCache.GetOrCreateAsync(data.id, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                return Task.FromResult(data);
            });
            return data.id;
        }

    }
}