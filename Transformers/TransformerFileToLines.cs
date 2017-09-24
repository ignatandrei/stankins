using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformerFileToLines : ITransform
    {
        public TransformerFileToLines()
        {
            Name = $"load file lines from FullName field";
        }
        public IRow[] valuesRead { get; set ; }
        public IRow[] valuesTransformed { get; set ; }
        public string Name { get ; set ; }
        public bool TrimEmptyLines { get; set; }
        async Task<IRow[]> LoadFromFile(IRow item)
        {
            List<IRow> r = new List<IRow>();
            if (!item.Values.ContainsKey("FullName"))
            {
                //TODO: log
                return null;
            }
            var file = item.Values["FullName"]?.ToString();
            if (!File.Exists(file))
            {
                //TODO: log
                return null;
            }
            int id = 1;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (TrimEmptyLines && string.IsNullOrWhiteSpace(line.Trim()))
                    {
                        continue;
                    }
                    var rr = new RowRead();
                    foreach (var val in item.Values)
                    {
                        rr.Values.Add(val.Key, val.Value);
                    }                    
                    rr.Values["text"] = line;
                    rr.Values["lineNr"] = id++;
                    r.Add(rr);
                }
            }
            return r.ToArray();

            
        }
        public async Task Run()
        {
            List<IRow> r = new List<IRow>();
            
            var all = new List<Task<IRow[]>>();
            foreach (var item in valuesRead)
            {
                all.Add(LoadFromFile(item));
            }
            var rec = new TaskExecutedList();
            all.ForEach(it=>rec.Add(new TaskExecuted(it)));
            await Task.WhenAll(rec.AllTasksWithLogging());
            foreach(var item in all.Where(it => it.Status== TaskStatus.RanToCompletion)){

                if ((item.Result?.Length??0) <1)
                    continue;

                r.AddRange(item.Result);
            }
            valuesTransformed = r.ToArray();
        }
    }
}
