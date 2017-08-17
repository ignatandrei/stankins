using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformRow: ITransform
    {
        public string TheExpression { get; set; }
        public TransformRow(string expression)
        {
            TheExpression = expression;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }

        public async Task Run()
        {
            var script = CSharpScript.Create<Dictionary<string,object>>(TheExpression,
                globalsType: typeof(IRow),
                options:ScriptOptions.Default.AddReferences(
                    //todo: load at serialize time
                    typeof(RowRead).GetTypeInfo().Assembly                    
                    ).AddImports("System")

                );
            
            script.Compile();
            valuesTransformed = new IRow[valuesRead.Length];
            int i = 0;
            foreach (var item in valuesRead)
            {
                var state = await script.RunAsync(item);
                var returnValue = state.ReturnValue;
                valuesTransformed[i] = item;
                valuesTransformed[i++].Values = returnValue;
            }
        }
    }
}