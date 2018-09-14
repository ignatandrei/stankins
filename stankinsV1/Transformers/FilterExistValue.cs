using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterExistValue : IFilter
    {
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

        public FilterExistValue(string key)
        {
            Name = "Filter exist value for {key}";
            Key = key;
        }

        public async Task Run()
        {
            var ret = new List<IRow>();
            foreach (var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                    continue;
                if (string.IsNullOrWhiteSpace(item.Values[Key]?.ToString()))
                    continue;

                ret.Add(item);
            }
            valuesTransformed = ret.ToArray();
        }
    }
}