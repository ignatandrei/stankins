using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace Transformers
{
    public class TransformerGroupRelationalString: TransformerGroupRelational<string>
    {
        public TransformerGroupRelationalString(string key):base(key)
        {

        }
    }
    public abstract class TransformerGroupRelational<T> : ITransform
        where T: IEquatable<T>
    {
        public TransformerGroupRelational(string key)
        {
            Key = key;            
        }
        public IRow[] valuesRead { get ; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get ; set ; }
        public string Key { get; set; }        

        public async Task Run()
        {
            
            var dict = new Dictionary<T, List<RowReadRelation>>();
            foreach(var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                {
                    string message = $"item values does not contain {Key}";
                    //@class.Log(LogLevel.Information, 0, $"transformer group relational: {message}", null, null);                        
                    message += "";
                    continue;
                }
                var val = (T)item.Values[Key];
                if (val == null)
                {
                    string message = $"val is null for {Key}";
                    //@class.Log(LogLevel.Information, 0, $"transformer group relational: {message}", null, null);                        
                    message += "";
                    continue;
                }
                if (!dict.ContainsKey(val))
                {
                    dict.Add(val, new List<RowReadRelation>());
                }
                var list = dict[val];
                var rr = new RowReadRelation();
                rr.AddValuesFrom(item);
                list.Add(rr);
            }
            var ret = new List<RowReadRelation>();
            foreach(var item in dict)
            {
                var rr = new RowReadRelation();
                rr.Values.Add(Key, item);
                var list = new List<IRowReceiveRelation>();
                rr.Relations.Add("childs", list);
                foreach(var val in item.Value)
                {
                    list.Add(val);
                }
                ret.Add(rr);
            }
            valuesTransformed = ret.ToArray();
            await Task.CompletedTask;
        }
    }
}
