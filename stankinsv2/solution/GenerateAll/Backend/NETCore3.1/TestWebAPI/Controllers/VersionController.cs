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
    public class VersionController : ControllerBase
    {

        public string VersionGenerator(){
            return "1.2020.107.28";
        }
        public string NetCoreVersion(){
            return "3.1";
        }
        public string AngularVersion(){
            return "10.1";
        }
    }
}