using MediaTransform;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Transformers
{
    
    public abstract class FilterRemovePropertyMaxMin<T> : IFilter
        where T : IComparable<T>

    {
        public string Name { get; set; }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string FieldName { get; set; }
        public GroupingFunctions RemovePropertyFunction { get; set; }

        public FilterRemovePropertyMaxMin(string fieldName, GroupingFunctions removePropertyFunction)
        {

            FieldName = fieldName;
            RemovePropertyFunction = removePropertyFunction;
            Name = $"remove all rows with {fieldName} = {removePropertyFunction}";
        }



        public async Task Run()
        {
            var len = valuesRead?.Length;
            var ret = new List<IRow>();
            if (len == 0)
                return;

            if (RemovePropertyFunction == GroupingFunctions.None)
            {
                valuesTransformed = valuesRead;
                return;
            }
            var mm = new MediaTransformMaxMin<T>();
            mm.FieldName = FieldName;
            mm.GroupFunction = RemovePropertyFunction;
            mm.valuesToBeSent = valuesRead;
            await mm.Run();
            T value = mm.Result;
            var type = typeof(T);
            //var vals = valuesRead.Select(it =>
            //    it.Values.ContainsKey(FieldName) ?
            //    it.Values[FieldName] : null)
            //    .Where(it => it != null)
            //    .Select(it => (T)Convert.ChangeType(it, type))
            //    .ToArray();
            //T value;
            //switch (RemovePropertyFunction)
            //{
            //    case GroupingFunctions.Min:
            //        value = vals.Min();
            //        break;
            //    case GroupingFunctions.Max:
            //        value = vals.Max();
            //        break;
            //    default:
            //        throw new ArgumentException($"cannot find {RemovePropertyFunction}");
            //}


            for (int i = len.Value - 1; i >= 0; i--)
            {
                var item = valuesRead[i];
                if (!item.Values.ContainsKey(FieldName))
                    continue;
                var valueLoop = (T)Convert.ChangeType(item.Values[FieldName], type);
                if (valueLoop.CompareTo(value) != 0)
                {
                    ret.Add(item);

                }
            }

            valuesTransformed = ret.ToArray();

            await Task.CompletedTask;
        }

    }

    public class FilterRemovePropertyMaxMinDateTime : FilterRemovePropertyMaxMin<DateTime>
    {
        public FilterRemovePropertyMaxMinDateTime(string fieldName, GroupingFunctions removePropertyFunction)
            :base(fieldName,removePropertyFunction)
        {
            
           
        }
    }
}
