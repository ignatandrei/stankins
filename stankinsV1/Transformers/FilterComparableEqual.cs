using System;

namespace Transformers
{
    public class FilterComparableEqual: FilterComparable
    {
        public FilterComparableEqual(Type comparableType, object value, string fieldName) : base(comparableType, value, fieldName, CompareValues.Equal)
        {

        }
    }
}
