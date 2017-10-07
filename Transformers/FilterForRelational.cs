using StankinsInterfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterForRelational : IFilter
    {
        public FilterForRelational(IFilter filter)
        {
            Filter = filter;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public IFilter Filter { get; set; }

        public async Task Run()
        {
            var data=valuesRead.Select(i => i as IRowReceiveRelation).Where(it=>it!=null).ToArray();
            await ApplyFilter(data);


        }
        async Task ApplyFilter(IRowReceiveRelation[] rr)
        {
            foreach(var item in rr)
            {
                foreach (var rel in item.Relations)
                {
                    await ApplyFilter(rel.Value.ToArray());
                    Filter.valuesRead = rel.Value.ToArray();
                    await Filter.Run();
                    rel.Value.Clear();
                    rel.Value.AddRange(Filter.valuesTransformed.Select(i=> i as IRowReceiveRelation).ToArray());
                }
                
                }
            }
        }
    }

