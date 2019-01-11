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
        public Dictionary<string,Dictionary<string,string>>[] ExecutorsDynamic { get; set; }
        private CRONExecution[] ExecutorsCache;
        public void CreateExecutors()
        {
            ExecutorsCache = new CRONExecution[ExecutorsDynamic.Length];
            int i = 0;
            foreach (var item in ExecutorsDynamic)
            {
                //TODO: ascertain exists item["Data"]["Type"]
                var type = Type.GetType(item["Data"]["Type"]);
                if (type == null)
                {
                    //TODO: log
                    continue;
                }
                if (!(Activator.CreateInstance(type) is CRONExecution instance))
                {
                    //TODO: log
                    continue;
                }
                foreach (var key in item["Data"].Keys)
                {
                    if (key == "Type")
                        continue;
                    type.GetProperty(key).GetSetMethod().Invoke(instance, new object[] { item["Data"][key] });
                }
                var cd = new CustomData();
                cd.UserName = this.UserName;
                cd.Tags = item["CustomData"]["Tags"].Split(',').Select(it=>it.Trim()).ToArray();
                cd.Name = item["CustomData"]["Name"];
                cd.Icon = item["CustomData"]["Icon"];
                instance.CustomData = cd;

                ExecutorsCache[i++] = instance;

            }
        }
        private CRONExecution[] Executors
        {
            get
            {
                if (ExecutorsCache != null)
                    return ExecutorsCache;

                CreateExecutors();
                return ExecutorsCache;
            }
        }
        public IEnumerable<IToBaseObjectExecutable> ToExecuteCRON()
        {
            var date = DateTime.UtcNow;
            return Executors.Where(it => it.ShouldRun(date));
            
        }

        public ResultWithData DataFromResult(AliveResult it)
        {

            CRONExecution c;
            CustomData cd;
            switch (it.Process.ToLower())
            {
                case "ping":
                    var p1 =Executors.First(p =>(p as PingAddress)?.NameSite == it.To);
                    c = p1;
                    cd = p1.CustomData;
                    break;
                case "webrequest":
                    var w1 = Executors.First(w => (w as WebAdress)?.URL == it.To);
                    c = w1;
                    cd = w1.CustomData;
                    break;
                case "receiverdatabaseserver":
                    var r1 = Executors.First(r => (r as DatabaseConnection)?.ConnectionString == it.To);
                    c = r1;
                    cd = r1.CustomData;
                    break;
                case "process":
                    var pr1 = Executors.First(p =>
                    {
                        var r = (p as StartProcess);
                        if (r == null)
                            return false;
                        return r.FileName + r.Parameters == it.To;

                    });
                    c = pr1;
                    cd = pr1.CustomData;
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
                CRONExecution = res,
                MyType = c.baseObject().GetType().FullName

            };

        }

    }
}
