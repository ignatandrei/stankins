using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace StankinsDataWeb.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion( "1.0" )]
    [ApiController]
    public class ReceiveDataController : ControllerBase
    {
        [HttpPost]
        public bool ReceiveData(dynamic data)
        {
            System.Console.WriteLine("receiving at "+ DateTime.Now.ToString("yyyyMMddHHmmss.fff"));
            var str=Convert.ToString(data);
            System.Console.WriteLine(str);
            System.Console.WriteLine("end receiving "+ DateTime.Now.ToString("yyyyMMddHHmmss.fff"));
            
            return true;
        }
    }
}