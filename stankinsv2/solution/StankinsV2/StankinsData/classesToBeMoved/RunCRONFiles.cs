using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static StankinsDataWeb.classesToBeMoved.CronExecution;

namespace StankinsDataWeb.classesToBeMoved
{

    /// <summary>
    /// shameless copy from https://devblogs.microsoft.com/pfxteam/asynclazyt/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        { }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
    public class RunCRONFiles : BackgroundService
    {
        private readonly CronExecutionFileWithCRON[] files;

        public RunCRONFiles(IHostingEnvironment hosting)
        {
            string dirPath = hosting.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            files = Directory.GetFiles(dirPath)
                .Select(it => new { name = Path.GetFileNameWithoutExtension(it), content = File.ReadAllText(it) })
                .Select(it => new CronExecutionFileWithCRON(it.name, it.content))
                .ToArray();

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var toExecTask = new ConcurrentDictionary<string, AsyncLazy<bool>>();
            while (!stoppingToken.IsCancellationRequested)
            {
                
                foreach (CronExecutionFileWithCRON item in files)
                {
                    if (item.ShouldRun(DateTime.Now))
                    {
                        var itemCache=item;
                        
                        toExecTask.TryAdd(item.Name,new AsyncLazy<bool>(()=> {
                            return itemCache.execute(); 
                            }));

                    }
                    await Task.WhenAny(toExecTask.Values.Select(it=>it.Value).ToArray());
                    var remove = new List<string>();
                    foreach(var fileItem in toExecTask)
                    {
                        //TODO: make a class to make it easy to understand this line
                        if (fileItem.Value.Value.IsCompleted)
                        {
                            Console.WriteLine($"was executed {fileItem.Key} with value {fileItem.Value.Value.Result}");
                            remove.Add(fileItem.Key);
                        }                        
                    }
                    foreach(var name in remove)
                    {
                        toExecTask.Remove(name,out _);
                    }

                }
                //TODO: make a proper log
                Console.WriteLine($"remains to be executed {toExecTask.Count}");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
