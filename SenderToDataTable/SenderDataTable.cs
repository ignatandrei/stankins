using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SenderToDataTable
{
    public class SenderDataTable : ISend
    {
        public SenderDataTable()
        {
            Name = "Send to DataTable";
        }
        public IRow[] valuesToBeSent { set; get; }
        public string Name { get ; set ; }
        public DataTable result;
        public async Task Send()
        {
            var keys = valuesToBeSent
                .SelectMany(it => it.Values)
                .Select(it=>it.Key)                
                .Distinct()
                .ToArray();
            var cols = keys
                .Select(k => new DataColumn(k, typeof(string)))
                .ToArray();
            result= new DataTable();
            result.Columns.AddRange(cols);
            foreach(var item in valuesToBeSent)
            {
                var vals = new List<string>();
                foreach (var k in keys)
                {
                    
                    if (item.Values.ContainsKey(k))
                    {
                        vals.Add(item.Values[k]?.ToString());
                    }
                    else
                    {
                        vals.Add(null);
                    }
                }
                result.Rows.Add(vals);
            }
        }
    }
}
