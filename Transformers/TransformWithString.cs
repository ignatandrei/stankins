using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{

    public class WithT<T>
    {
        public T oldValue;
    }
    public class TransformOneValueWithStringGeneral : TransformOneValueWithString<object, object>
    {
        public TransformOneValueWithStringGeneral(string expression, string oldField, string newField) : base(expression, oldField, newField)
        {
        }
    }
    public class TransformOneValueWithString<T, U> : ITransform
    {
        public string TheExpression { get; set; }
        public string OldField { get; set; }
        public string NewField { get; set; }
        public TransformOneValueWithString(string expression, string oldField, string newField)
        {
            TheExpression = expression;
            OldField = oldField;
            NewField = newField;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }

        public async Task Run()
        {
            
            var val = new WithT<T>();
            val.oldValue = default(T);
            var script = CSharpScript.Create<U>(TheExpression,               
                globalsType: typeof(WithT<T>)
                );
            script.Compile();
            foreach (var item in valuesRead)
            {
                var data = item.Values[OldField];
                T oldValue = (T)Convert.ChangeType(data, typeof(T));
                val.oldValue = oldValue;
                //TODO: catch exception
                var state = await script.RunAsync(val);
                var returnValue = state.ReturnValue;
                //var val= await CSharpScript.EvaluateAsync<U>( TheExpression, globals: data, globalsType:data.GetType());
                if (item.Values.ContainsKey(NewField))
                {
                    item.Values[NewField] = returnValue;
                }
                else
                {
                    item.Values.Add(NewField, returnValue);
                }
            }
            valuesTransformed = valuesRead;
        }
    }
}
