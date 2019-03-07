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
        static bool InternalRequestRestart;
        public async static Task Main(string[] args)
        {
            do{
                InternalRequestRestart=false;
                cancel= new CancellationTokenSource();
                await CreateWebHostBuilder(args).Build().RunAsync(cancel.Token);
                await Task.Delay(10);//just waiting some time
                Console.WriteLine("restarting");
            }while(InternalRequestRestart);

        }
        public static void Shutdown()
        {
            InternalRequestRestart=true;
            cancel.Cancel();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
