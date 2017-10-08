using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterExcludeRelation : ITransform
    {
        public FilterExcludeRelation(params string[] excludeRelations)
        {
            ExcludeRelations = excludeRelations;
            var all = string.Join(",", excludeRelations);
            Name = $"exclude relational {all}";
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string[] ExcludeRelations { get; set; }
        bool existsProperties;
        void RemoveRelational(IRowReceiveRelation rr)
        {
            if (rr == null)
                return;

            var keys = rr.Relations.Keys.ToArray();
            if (existsProperties)
            {
                foreach (var item in keys.Intersect(ExcludeRelations))
                {
                    rr.Relations.Remove(item);
                }
            }



            foreach (var item in rr.Relations.SelectMany(it => it.Value))
            {
                RemoveRelational(item);
            }

        }
        public async Task Run()
        {
            existsProperties = (ExcludeRelations?.Length??0) > 0;
            foreach (var val in valuesRead)
            {
                var data = val as IRowReceiveRelation;
                RemoveRelational(data);

            }
            valuesTransformed = valuesRead;
        }
    }
}
