using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterComparable : ITransform
        
    {
        public IRow[] valuesRead { get; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public Type ComparableType { get; set; }
        public object Value { get; set; }
        public string FieldName { get; set; }
        public CompareValues HowToCompareValues { get; set; }

        public FilterComparable(Type comparableType, object value, string fieldName, CompareValues compareValues)
        {
            ComparableType = comparableType;
            Value = value;
            FieldName = fieldName;
            HowToCompareValues = compareValues;
        }
        public async Task Run()
        {
            IComparable v = (IComparable)Value;
            var returnValues = new List<IRow>();
            foreach(var item in valuesRead)
            {
                var val = item.Values[FieldName];
                var valueLoop = Convert.ChangeType(val, ComparableType);
               
                var res = (v.CompareTo(valueLoop));
                bool add = false;
                switch (HowToCompareValues)
                {
                    case CompareValues.Equal:
                        add = (res == 0);
                        break;
                    case CompareValues.Less:
                        add = (res > 0);
                        break;
                    case CompareValues.Greater:
                        add = (res < 0);
                        break;
                    default:
                        //TODO: add implementation for LessOrEqual or GreaterOrEqual
                        throw new ArgumentException("This is not implemented :" + HowToCompareValues);
                        

                }
                if (!add)
                    continue;
                returnValues.Add(item);
                

            }
            valuesTransformed = returnValues.ToArray();

            await Task.CompletedTask;
        }
    }
}
