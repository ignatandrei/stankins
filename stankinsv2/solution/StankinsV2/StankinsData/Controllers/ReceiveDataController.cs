using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StankinsDataWeb.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion( "1.0" )]
    [ApiController]
    public class ReceiveDataController : ControllerBase
    {
        [HttpPost]
        public async Task<bool> ReceiveData(dynamic data)
        {
            return true;
        }
    }
}