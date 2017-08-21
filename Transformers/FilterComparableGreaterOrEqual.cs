using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    public class FilterComparableGreaterOrEqual: FilterComparable
    {
        public FilterComparableGreaterOrEqual(Type comparableType, object value, string fieldName):base(comparableType,value,fieldName,CompareValues.GreaterOrEqual)
        {

        }
    }
}
