using StankinsInterfaces;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformAddNewField : ITransform
    {
        public TransformAddNewField(string key)
        {

            Key = key;
            Name = $"adding field {Key} as latest value if not exists";
            Value = null;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public bool Increment { get; set; }
        public async Task Run()
        {
            if (Increment)
            {
                int res;
                if (!int.TryParse(Value?.ToString(), out res))
                    res = 0;

                Value = res++;
            }
            //object previousValue = null;
            foreach (var item in valuesRead)
            {
                if (item.Values.ContainsKey(Key))
                {
                    //previousValue = item.Values[Key];
                    continue;
                }
                item.Values.Add(Key, Value);
                if (Increment)
                {
                    Value = int.Parse(Value.ToString())+1;
                }
            }
            valuesTransformed = valuesRead;
        }
    }
}
