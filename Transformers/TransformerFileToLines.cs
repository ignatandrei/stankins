using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformerFileToLines : ITransform
    {
        public IRow[] valuesRead { get; set ; }
        public IRow[] valuesTransformed { get; set ; }
        public string Name { get ; set ; }
        public bool TrimEmptyLines { get; set; }
        public async Task Run()
        {
            List<IRow> r = new List<IRow>();
            foreach(var item in valuesRead)
            {
                if (!item.Values.ContainsKey("FullName"))
                {
                    //TODO: log
                    continue;
                }
                var file = item.Values["FullName"]?.ToString();
                if (!File.Exists(file))
                {
                    //TODO: log
                    continue;
                }
                //TODO: use async
                var lines = File.ReadAllLines(file);
                int i = 0;
                var length = lines.Length;
                var padding = length.ToString().Length;
                foreach(var line in lines)
                {
                    #region padding
                    //todo: use padleft
                    string id = (++i).ToString();
                    while (id.ToString().Length < padding)
                    {
                        id = "0" + id;
                    }
                    #endregion
                    if (TrimEmptyLines && string.IsNullOrWhiteSpace(line.Trim()))
                    {
                        continue;
                    }
                    var rr = new RowRead();
                    rr.Values.Add($"line{id}", line);
                    rr.Values.Add($"text", line);
                    rr.Values.Add("FullName", file);
                    r.Add(rr);
                }
            }
            valuesTransformed = r.ToArray();
        }
    }
}
