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
        public FilterExcludeRelation()
        {
            ExcludeProperties = new List<string>();
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public List<string> ExcludeProperties { get; set; }
        bool existsProperties;
        void RemoveRelational(IRowReceiveRelation rr)
        {
            if (rr == null)
                return;

            var keys = rr.Relations.Keys.ToArray();
            if (existsProperties)
            {
                foreach (var item in keys.Intersect(ExcludeProperties))
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
            existsProperties = (ExcludeProperties?.Count > 0);
            foreach (var val in valuesRead)
            {
                var data = val as IRowReceiveRelation;
                RemoveRelational(data);

            }
            valuesTransformed = valuesRead;
        }
    }
}
