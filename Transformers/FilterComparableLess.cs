using System;

namespace Transformers
{
    public class FilterComparableLess : FilterComparable
    {
        public FilterComparableLess(Type comparableType, object value, string fieldName):base(comparableType,value,fieldName,CompareValues.Less)
        {

        }
    }
}
