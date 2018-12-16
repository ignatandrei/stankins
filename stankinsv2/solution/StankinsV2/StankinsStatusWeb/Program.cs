using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StankinsStatusWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var t = Type.GetType("StankinsStatusWeb.PingAddress, StankinsAliveMonitor");
            var p = new PingAddress();
            var s = p.GetType().FullName;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
