using StankinsInterfaces;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterForHierarchical : IFilter
    {
        public FilterForHierarchical(IFilter filter)
        {
            Filter = filter;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public IFilter Filter { get; set; }

        public async Task Run()
        {
            Filter.valuesRead = valuesRead;
            await Filter.Run();
            valuesTransformed = Filter.valuesTransformed;
        }
    }
}
