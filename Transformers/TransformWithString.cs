using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using StankinsInterfaces;
using StanskinsImplementation;
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
    public class TransformerFieldStringInt : TransformOneValueGeneral
    {
        public TransformerFieldStringInt(string oldField, string newField) : base("int.Parse((oldValue??\"0\").ToString())", oldField, newField)
        {

        }
    }
    public class TransformOneValueGeneral : TransformOneValue<object, object>
    {
        public TransformOneValueGeneral(string expression, string oldField, string newField) : base(expression, oldField, newField)
        {
        }
    }
    public class TransformOneValue<T, U> : ITransform
    {
        public string TheExpression { get; set; }
        public string OldField { get; set; }
        public string NewField { get; set; }
        public TransformOneValue(string expression, string oldField, string newField)
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
                globalsType: typeof(WithT<T>),
                options: ScriptOptions.Default.AddReferences(
                    //todo: load at serialize time
                    typeof(RowRead).GetTypeInfo().Assembly
                    ).AddImports("System")
                );
            script.Compile();
            if (valuesRead == null)
            {
                //TODO: log
                return;
            }
            foreach (var item in valuesRead)
            {
                if (!item.Values.ContainsKey(OldField))
                {
                    throw new ArgumentException($"values does not contain {OldField}");
                }
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
