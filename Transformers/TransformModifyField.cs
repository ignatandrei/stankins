using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformModifyField : ITransform
    {
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string FormatString { get; set; }

        public TransformModifyField(string key,string formatString)
        {
            Name = $"transform {key} with {formatString}";
            Key = key;
            FormatString = formatString;
        }

        public async Task Run()
        {
            valuesTransformed = valuesRead;
            foreach (var item in valuesTransformed)
            {
                //TODO: see if implements IFormattable
                item.Values[Key] = string.Format(FormatString, item.Values[Key]);
            }
            await Task.CompletedTask;
        }
    }
}

