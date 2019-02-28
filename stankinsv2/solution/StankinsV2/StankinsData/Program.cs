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
        static readonly CancellationTokenSource cancel = new CancellationTokenSource();
        
        public async static Task Main(string[] args)
        {
            await CreateWebHostBuilder(args).Build().RunAsync(cancel.Token);

        }
        public static void Shutdown()
        {
            cancel.Cancel();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
