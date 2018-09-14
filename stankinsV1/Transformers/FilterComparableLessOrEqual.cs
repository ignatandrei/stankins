using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    public class FilterComparableLessOrEqual : FilterComparable
    {
        public FilterComparableLessOrEqual(Type comparableType, object value, string fieldName):base(comparableType,value,fieldName,CompareValues.LessOrEqual)
        {

        }
    }
}
