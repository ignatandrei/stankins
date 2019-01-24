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
            var allExecutors  = new List<CRONExecution>();
            
            var mult = typeof(CRONExecutionMultiple<>);
            foreach (var item in ExecutorsDynamic)
            {
                //TODO: ascertain exists item["Data"]["Type"]
                var type = Type.GetType(item["Data"]["Type"]);
                if (type == null)
                {
                    //TODO: log
                    continue;
                }
                var crons = new List<CRONExecution>();


                var t = Activator.CreateInstance(type);
                if (t is CRONExecution)
                {
                    foreach (var key in item["Data"].Keys)
                    {
                        if (key == "Type")
                            continue;
                        type.GetProperty(key).GetSetMethod().Invoke(t, new object[] { item["Data"][key] });
                    }
                    crons.Add(t as CRONExecution);
                }
                else
                {
                    var n = t as CRONExecutionMultiple<CRONExecution>;
                    if (n != null)
                    {
                        foreach (var key in item["Data"].Keys)
                        {
                            if (key == "Type")
                                continue;
                            type.GetProperty(key).GetSetMethod().Invoke(n, new object[] { item["Data"][key] });
                        }
                        crons.AddRange(n.Multiple().ToArray());

                    }
                }



                if (crons.Count == 0)
                {
                    //TODO: log
                    continue;
                }
                foreach (var instance in crons)
                {

                    var cd = new CustomData();
                    cd.UserName = this.UserName;
                    var custom = item["CustomData"];
                    if (custom.ContainsKey("Tags"))
                    {
                        cd.Tags = custom["Tags"].Split(',').Select(it => it.Trim()).ToArray();

                    }
                    if (custom.ContainsKey("Name"))
                    {
                        cd.Name = custom["Name"];
                    }
                    if (custom.ContainsKey("Icon"))
                    {
                        cd.Icon = custom["Icon"];
                    }
                    instance.CustomData = cd;
                    allExecutors.Add(instance);
                }
                ExecutorsCache = allExecutors.ToArray();

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
            return Executors.Where(it =>
            {
                try
                {
                    return it.ShouldRun(date);
                }
                catch (Exception ex)
                {
                    var x = it;
                    var s = ex.Message;
                    return false;
                }
            });
            
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
