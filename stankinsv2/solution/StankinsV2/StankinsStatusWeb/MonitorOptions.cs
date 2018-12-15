using Microsoft.Extensions.Options;
using Stankins.Alive;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StankinsStatusWeb
{
    public class MonitorOptions
    {
        public string UserName { get; set; }
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
            cd.UserName = this.UserName;
            return new ResultWithData()
            {
                AliveResult = it,
                CustomData = cd,
                CRONExecution = res

            };

        }

    }
}
