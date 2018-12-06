using Cronos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Stankins.Alive;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class MonitorOptions
    {
        public WebAdress[] WebAdresses { get; set; }
        public PingAddress[] PingAddresses { get; set; }
        public DatabaseConnection[] Databases { get; set; }
        public IEnumerable<IToBaseObject> AllItems()
        {
            return ((IToBaseObject[])WebAdresses)
                .Union(((IToBaseObject[])PingAddresses))
                .Union(((IToBaseObject[])Databases));

        }
        public IEnumerable<IToBaseObject> ToExecuteCRON()
        {
            var date = DateTime.UtcNow;

            foreach (var item in AllItems())
            {
                var cron = item as CRONExecution;
                if (cron.ShouldRun(date))
                {
                    yield return item;

                }
            }
        }

        public ResultWithData DataFromResult(AliveResult it)
        {

            CronExecutionBase c;
            CustomData cd;
            switch (it.Process.ToLower())
            {
                case "ping":
                    var p1 = PingAddresses.First(p => p.NameSite == it.To);
                    c = p1;
                    cd = p1.CustomData;
                    break;
                case "webrequest":
                    var w1 = WebAdresses.First(w => w.URL == it.To);
                    c = w1;
                    cd = w1.CustomData;
                    break;
                case "receiverdatabaseserver":
                    var r1 = Databases.First(w => w.ConnectionString == it.To);
                    c = r1;
                    cd = r1.CustomData;
                    break;
                default:
                    throw new ArgumentException($"not a good process {it.Process.ToLower()}");
            }
            var res = new CronExecutionBase();
            res.CopyFrom(c);
            return new ResultWithData()
            {
                AliveResult = it,
                CustomData = cd,
                CRONExecution = res

            };

        }

    }
    public interface IToBaseObject
    {
        BaseObject baseObject();
        Task<DataTable> Execute();
    }
    [DebuggerDisplay("{CRON} {LastRunTime} {NextRunTime}" )]
    public class CronExecutionBase
    {
        public string CRON { get; set; }


        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public void CopyFrom(CronExecutionBase c)
        {
            this.CRON = c.CRON;
            this.LastRunTime = c.LastRunTime;
            this.NextRunTime = c.NextRunTime;
            
        }
    }
    public class CRONExecution: CronExecutionBase 
    {
       
       

        
        void MakeNextExecute()
        {
            LastRunTime = NextRunTime;
            //todo: cache this
            var expression = CronExpression.Parse(CRON, CronFormat.IncludeSeconds);
            NextRunTime= expression.GetNextOccurrence(DateTime.UtcNow);
        }
        public bool ShouldRun(DateTime currentTime)
        {
            if (NextRunTime == null && LastRunTime == null)
            {
                MakeNextExecute();
                return true;//execute once
            }
            if (NextRunTime == null)
                return false;

            if (NextRunTime < currentTime)
            {
                MakeNextExecute();
                return true;
            }
            return false;
        }
    }
    public class CustomData
    {
        public string Name { get; set; }
        public string[] Tags { get; set; }
    }
    public class WebAdress: CRONExecution, IToBaseObject
    {
        public string URL { get; set; }
        public CustomData CustomData { get; set; }


        public BaseObject baseObject()
        {
            return new  ReceiverWeb(URL);
        }

        public async Task<DataTable> Execute()
        {
          
            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }

        
    }
    public class ResultWithData : INotification
    {
        public AliveResult AliveResult { get; set; }
        public CustomData CustomData { get; set; }
        public CronExecutionBase CRONExecution { get; set; }
    }
    public class DatabaseConnection : CRONExecution, IToBaseObject
    {
        public CustomData CustomData { get; set; }
        public string ConnectionString { get; set; }
        public string TypeOfReceiver { get; set; }
        public BaseObject baseObject()
        {
            var type = Type.GetType(TypeOfReceiver);
            return Activator.CreateInstance(type,ConnectionString) as BaseObject;
        }
        public async Task<DataTable> Execute()
        {

            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }
    }
    public class PingAddress : CRONExecution, IToBaseObject
    {
        public CustomData CustomData { get; set; }
        public string NameSite { get; set; }
        
        public BaseObject baseObject()
        {
            return new ReceiverPing(NameSite);
        }

        public async Task<DataTable> Execute()
        {
            
            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }


    }
    public class RunTasks : BackgroundService
    {
        private readonly MonitorOptions opt;
        private readonly IServiceScopeFactory sc;

        public RunTasks(MonitorOptions opt, IServiceScopeFactory sc)
        {
            this.opt = opt;
            this.sc = sc;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                var toExec = opt.ToExecuteCRON().Select(it=>it.Execute()).ToArray();
                if (toExec.Length > 0)
                {

                    var checkedDate = DateTime.UtcNow;
                    var res = await Task.WhenAll(toExec);
                    var dataToBeSent = res
                      
                        .Select(it => AliveStatus.FromTable(it))
                        .SelectMany(it=>it)
                        .Select(it=>opt.DataFromResult(it))                     
                        .ToArray() ;

                  

                    using (var scope = sc.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        foreach (var item in dataToBeSent)
                        {
                            await mediator.Publish(item);
                        }
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
