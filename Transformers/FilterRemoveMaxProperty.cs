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
    public enum FilterRemovePropertyFunction
    {
        None = 0,
        Max = 1,
        Min = 2
    }
    public abstract class FilterRemovePropertyMaxMin<T> : IFilter
        where T : IComparable<T>

    {
        public string Name { get; set; }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string FieldName { get; set; }
        public FilterRemovePropertyFunction RemovePropertyFunction { get; set; }

        public FilterRemovePropertyMaxMin(string fieldName, FilterRemovePropertyFunction removePropertyFunction)
        {

            FieldName = fieldName;
            RemovePropertyFunction = removePropertyFunction;
        }



        public async Task Run()
        {
            var len = valuesRead?.Length;
            var ret = new List<IRow>();
            if (len == 0)
                return;

            if (RemovePropertyFunction == FilterRemovePropertyFunction.None)
            {
                valuesTransformed = valuesRead;
                return;
            }
            var type = typeof(T);
            var vals = valuesRead.Select(it =>
                it.Values.ContainsKey(FieldName) ?
                it.Values[FieldName] : null)
                .Where(it => it != null)
                .Select(it => (T)Convert.ChangeType(it, type))
                .ToArray();
            T value;
            switch (RemovePropertyFunction)
            {
                case FilterRemovePropertyFunction.Min:
                    value = vals.Min();
                    break;
                case FilterRemovePropertyFunction.Max:
                    value = vals.Max();
                    break;
                default:
                    throw new ArgumentException($"cannot find {RemovePropertyFunction}");
            }


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
        public FilterRemovePropertyMaxMinDateTime(string fieldName, FilterRemovePropertyFunction removePropertyFunction)
            :base(fieldName,removePropertyFunction)
        {

           
        }
    }
}
