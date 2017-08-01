using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transformers
{
    //replace with
    //https://github.com/davideicardi/DynamicExpresso
    public class TransformAddField<T,U> : ITransform
    {
        Func<T, U> transformFunc { get; set; }
        public string OldField { get; set; }
        public string NewField { get; set; }
        public TransformAddField(Func<T,U> func, string oldField, string newField)
        {
            transformFunc = func;
            OldField = oldField;
            NewField = newField;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }

        public async Task Run()
        {
            //var newVals = new List<IRow>();
            foreach (var item in valuesRead)
            {
                var data = item.Values[OldField];
                T convert = (T)Convert.ChangeType(data, typeof(T));
                var val = transformFunc(convert);
                if (item.Values.ContainsKey(NewField))
                {
                    item.Values[NewField] = val;
                }
                else
                {
                    item.Values.Add(NewField, val);
                }
                //newVals.Add(item);
            }
            valuesTransformed = valuesRead;
        }
    }
}
