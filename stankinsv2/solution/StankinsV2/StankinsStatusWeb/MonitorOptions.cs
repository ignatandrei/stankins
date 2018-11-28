using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Stankins.Alive;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class MonitorOptions
    {
        public WebAdress[] WebAdresses { get; set; }
        public PingAddress[] PingAddresses { get; set; } 

        public IEnumerable< BaseObject> ToExecute()
        {
            var date = DateTime.UtcNow;
            foreach(var item in WebAdresses)
            {

                if (item.ShouldRun(date))
                {
                    yield return item.baseObject();
                    
                }
            }
            //TODO: not copy paste
            foreach (var item in PingAddresses)
            {

                if (item.ShouldRun(date))
                {
                    yield return item.baseObject();
                   
                }
            }
        }

    }
    public interface IToBaseObject
    {
        BaseObject baseObject();
    }
    public class CRONExecution
    {
        public string CRON { get; set; }
       

        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
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
                return true;//execute once
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
    public class WebAdress: CRONExecution, IToBaseObject
    {
        public string URL { get; set; }
       

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

    public class PingAddress : CRONExecution, IToBaseObject
    {
      
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

        public RunTasks(MonitorOptions opt)
        {
            this.opt = opt;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var toExec = opt.ToExecute().ToArray();
                if (toExec.Length > 0)
                {
                    var t = new List<Task<IDataToSent>>();
                    foreach (var item in toExec)
                    {
                        t.Add(item.TransformData(null));
                    }

                    var res = await Task.WhenAll(t.ToArray());
                    var x = res.Length;
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
