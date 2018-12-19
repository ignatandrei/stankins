using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Stankins.Alive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class RunTasks : BackgroundService
    {
        
        private readonly IServiceScopeFactory sc;

        public RunTasks(IServiceScopeFactory sc)
        {
           
            this.sc = sc;
        }
        private async Task PublishData(MonitorOptions opt,DataTable res)
        {

            var dataToBeSent = AliveStatus.FromTable(res)
                           .Select(d => opt.DataFromResult(d))
                           .ToArray();
            using (var scope = sc.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                foreach (var item in dataToBeSent)
                {
                    await mediator.Publish(item);
                }
            }


        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MonitorOptions opt;
           
            //var existingNames = new List<string>();
            var toExecTask = new Dictionary<string,Task<DataTable>>();
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"starting monitor again at UTC {DateTime.UtcNow.ToString("o")}");
                using (var scope = sc.CreateScope())
                {
                    var snap = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<MonitorOptions>>();
                    opt = snap.Value;
                }
                
                var itemsToExec = opt.ToExecuteCRON()
                    .Where(it=> !toExecTask.ContainsKey(it.baseObject().Name))
                    .ToArray();
                foreach (var item in itemsToExec)
                {
                    var name = item.baseObject().Name;
                    if (toExecTask.ContainsKey(name))
                    {
                        //TODO: log this message and show
                        continue;
                    }
                    toExecTask.Add(name, item.Execute());
                    
                }
                //existingNames.AddRange(itemsToExec.Select(it => it.baseObject().Name).ToArray());
                Console.WriteLine(string.Join(',', toExecTask.Keys));

                if (toExecTask.Count > 0)
                {
                    await Task.WhenAny(toExecTask.Values.ToArray());
                    var remove = new List<string>();
                    foreach(var item in toExecTask)
                    {
                        if (item.Value.IsCompleted)
                        {
                            remove.Add(item.Key);
                        }
                        if (item.Value.IsCompletedSuccessfully)
                        {
                            
                            await PublishData(opt, item.Value.Result);
                        }
                    }
                    foreach(var item in remove)
                    {
                        toExecTask.Remove(item);
                    }
                    
                }                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            }
            //var next = cron.NextOccurence();
            //while (next != null)// exit when next is null
            //{
                
            //    await Task.Delay(DateTime.Now.Subtract(next.Value));
            //    await b.TransformData(null);
            //    //TODO: Send data
            //    next = cron.NextOccurence();
                
            //}
        }
    }
}
