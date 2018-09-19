using System;
using System.Collections.Generic;
using System.Text;

namespace StankinsCommon
{
    public class CompareStrings : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return 0 == (string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase));
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
                return 0;
            return obj.ToLower().GetHashCode();
        }
    }
    public class CtorDictionary: Dictionary<string, object>
    {
        public CtorDictionary():base(new CompareStrings())
        {

        }
        public CtorDictionary(IDictionary<string,object> data):base(data,new CompareStrings())
        {

        }
    }
}
