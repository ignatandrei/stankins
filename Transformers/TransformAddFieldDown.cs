using StankinsInterfaces;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformAddFieldDown : ITransform
    {
        public TransformAddFieldDown(string key)
        {
            
            Key = key;
            Name = $"adding field {Key} as latest value if not exists";
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }        
        public string Key { get; set; }

        public async Task Run()
        {
            object previousValue = null;
            foreach (var item in valuesRead)
            {
                if (item.Values.ContainsKey(Key))
                {
                    previousValue = item.Values[Key];
                    continue;
                }
                item.Values.Add(Key, previousValue);                

            }
            valuesTransformed = valuesRead;
        }
    }
}
