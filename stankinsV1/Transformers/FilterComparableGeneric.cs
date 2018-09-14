using System;

namespace Transformers
{
    public class FilterComparableGeneric<T> : FilterComparable
       where T : IComparable<T>
    {

        public FilterComparableGeneric(Type comparableType, object value, string fieldName, CompareValues compareValues) : base(comparableType, value, fieldName, compareValues)
        {
        }
    }
}
