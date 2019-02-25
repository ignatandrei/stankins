using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StankinsDataWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UtilsController : ControllerBase
    {
         [HttpGet]
        public string Ping()
        {
            return DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
        }
        [HttpGet]
        public FileVersionInfo[] GetVersions([FromServices] IHostingEnvironment hosting)
        {
            var dirPath = hosting.ContentRootPath;
            var ret = new List<FileVersionInfo>();
            var files = Directory.EnumerateFiles(dirPath, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.EnumerateFiles(dirPath, "*.exe", SearchOption.AllDirectories));
 
            foreach (string item in files)
            {
                try
                {
                    var info = FileVersionInfo.GetVersionInfo(item);
                    ret.Add(info);
 
                }
                catch (Exception)
                {
                    //TODO: log
                }
            }
            return ret.ToArray();
        }
    }
}