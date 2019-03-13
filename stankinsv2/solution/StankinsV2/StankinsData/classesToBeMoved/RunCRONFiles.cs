using Microsoft.Extensions.Hosting;
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
            Dictionary<string, Task<bool>> toExecTask = new Dictionary<string, Task<bool>>();
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"starting again at {DateTime.UtcNow}");
                foreach (CronExecutionFile item in files)
                {
                    if (item.ShouldRun(DateTime.UtcNow))
                    {
                        //CronExecutionFile itemCache = item;
                        if (!toExecTask.ContainsKey(item.Name))
                        {
                            toExecTask[item.Name] = item.execute();
                        }

                    }
                    else
                    {
                        Console.WriteLine($"{item.Name} => {item.NextRunTime}");
                        //item.reload();
                    }
                    if (toExecTask.Count > 0)
                    {
                        Console.WriteLine($" number of tasks to execute " + toExecTask.Count);

                        await Task.WhenAny(toExecTask.Values.Select(it => it).ToArray());
                        List<string> remove = new List<string>();
                        foreach (var fileItem in toExecTask)
                        {
                            //TODO: make a class to make it easy to understand this line
                            if (fileItem.Value.IsCompleted)
                            {
                                Console.WriteLine($"was executed {fileItem.Key} with value {fileItem.Value.Result}");
                                remove.Add(fileItem.Key);
                            }
                        }
                        foreach (string name in remove)
                        {
                            toExecTask.Remove(name, out _);
                        }
                    }
                }
                //TODO: make a proper log
                Console.WriteLine($"remains to be executed {toExecTask.Count}");
                await Task.Delay(TimeSpan.FromSeconds(10));

            }
        }
    }
}
