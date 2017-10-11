using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformClearValues:ITransform
    {
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public TransformClearValues()
        {
            Name = "Clear transform values";
        }

        public async Task Run()
        {
            valuesTransformed = null;
            await Task.CompletedTask;
        }
    }
}
