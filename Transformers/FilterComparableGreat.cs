using System;

namespace Transformers
{
    public class FilterComparableGreat : FilterComparable
    {
        public FilterComparableGreat(Type comparableType, object value, string fieldName) : base(comparableType, value, fieldName, CompareValues.Greater)
        {

        }
    }
}
