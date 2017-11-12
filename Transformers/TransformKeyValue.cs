using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformKeyValue : ITransform
    {
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        
        
        public TransformKeyValue()
        {
            Name = $"transform into key value";
            
        }

        public async Task Run()
        {
            int nrValues = valuesRead?.Length ?? 0;
            if (nrValues == 0)
                return ;
            valuesTransformed = new RowRead[nrValues];
            for (int i = 0; i < nrValues; i++)
            {
                var rr = new RowRead();
                foreach(var item in valuesRead[i].Values)
                {
                    if (item.Value == null)
                        continue;
                    if (string.IsNullOrWhiteSpace(item.Value.ToString()))
                        continue;
                    if (!rr.Values.ContainsKey("key")) {
                        rr.Values["key"] = item.Value;
                        continue;
                    }
                    if (rr.Values.ContainsKey("value"))
                    {
                        throw new ArgumentException("more than 1 value for " + item.Key);
                    }

                    rr.Values.Add("value",item.Value );
                    
                }
                valuesTransformed[i] = rr;
            }


        }
    }
}
