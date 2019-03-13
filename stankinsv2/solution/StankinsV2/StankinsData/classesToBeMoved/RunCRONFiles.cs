﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static StankinsDataWeb.classesToBeMoved.CronExecution;

namespace StankinsDataWeb.classesToBeMoved
{

    public class RunCRONFiles : BackgroundService
    {
        private readonly CronExecutionFile[] files;

        public RunCRONFiles(IHostingEnvironment hosting)
        {
            System.Console.WriteLine("starting cron files");
            string dirPath = hosting.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            files = Directory.GetFiles(dirPath)
                //.Select(it => new { name = Path.GetFileNameWithoutExtension(it), content = File.ReadAllText(it) })
                .Select(it => new CronExecutionFile(it))
                .ToArray();

        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var executing = new HashSet<string>();

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime dt = DateTime.UtcNow;
                Console.WriteLine($"starting again at {dt}");
                List<CronExecutionFile> f = files
                    .Where(it => it.ShouldRun(dt))
                    .Where(it => !executing.Contains(it.Name))
                    .ToList();
                if (f.Count > 0)
                {
                    foreach (var item in f)
                    {
                        executing.Add(item.Name);
                    }

                    var q = f.Select<CronExecutionFile, Func<Task<string>>>(it => async () =>
                    {
                        Console.WriteLine($"before execute {it.Name}");
                        await it.execute();
                        executing.Remove(it.Name);
                        return it.Name;
                    }).ToArray();
                    await Task.WhenAny(q.Select(it=>it()).ToArray());

                    Console.WriteLine($"remains to be executed {q.Length}");

                }

                //TODO: make a proper log
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            }
        }
    }
}
