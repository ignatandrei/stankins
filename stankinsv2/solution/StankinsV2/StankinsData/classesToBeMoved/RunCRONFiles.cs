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

   public sealed class AsyncLazy<T>
{
    /// <summary>
    /// The underlying lazy task.
    /// </summary>
    private readonly Lazy<Task<T>> instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="factory">The delegate that is invoked on a background thread to produce the value when it is needed.</param>
    public AsyncLazy(Func<T> factory)
    {
        instance = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="factory">The asynchronous delegate that is invoked on a background thread to produce the value when it is needed.</param>
    public AsyncLazy(Func<Task<T>> factory)
    {
        instance = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    /// <summary>
    /// Asynchronous infrastructure support. This method permits instances of <see cref="AsyncLazy&lt;T&gt;"/> to be await'ed.
    /// </summary>
    public TaskAwaiter<T> GetAwaiter()
    {
        return instance.Value.GetAwaiter();
    }

    /// <summary>
    /// Starts the asynchronous initialization, if it has not already started.
    /// </summary>
    public void Start()
    {
        var unused = instance.Value;
    }
}
    public class RunCRONFiles : BackgroundService
    {
        private readonly CronExecutionFile[] files;

        public RunCRONFiles(IHostingEnvironment hosting)
        {
            string dirPath = hosting.ContentRootPath;
            dirPath = Path.Combine(dirPath, "cronItems", "v1");
            files = Directory.GetFiles(dirPath)
                //.Select(it => new { name = Path.GetFileNameWithoutExtension(it), content = File.ReadAllText(it) })
                .Select(it => new CronExecutionFile(it))
                .ToArray();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var toExecTask = new ConcurrentDictionary<string, Task<bool>>();
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"starting again at {DateTime.UtcNow}");
                foreach (CronExecutionFile item in files)
                {
                    if (item.ShouldRun(DateTime.UtcNow))
                    {
                        CronExecutionFile itemCache = item;
                        if(!toExecTask.ContainsKey(item.Name))
                        if (toExecTask.TryAdd(item.Name,item.execute()))
                        {
                            Console.WriteLine($"scheduling {item.Name}");
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
                        foreach (KeyValuePair<string, Task<bool>> fileItem in toExecTask)
                        {
                            //TODO: make a class to make it easy to understand this line
                            if (fileItem.IsCompleted)
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
