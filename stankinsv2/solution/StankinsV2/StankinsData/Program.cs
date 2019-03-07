using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StankinsDataWeb
{
    public class Program
    {
        static CancellationTokenSource cancel ;
        static bool ExternalShutdown;
        public async static Task Main(string[] args)
        {
            do{
                ExternalShutdown=true;
                cancel= new CancellationTokenSource();
                await CreateWebHostBuilder(args).Build().RunAsync(cancel.Token);
                await Task.Delay(10);//just waiting some time
            }while(ExternalShutdown);

        }
        public static void Shutdown()
        {
            ExternalShutdown=false;
            cancel.Cancel();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
