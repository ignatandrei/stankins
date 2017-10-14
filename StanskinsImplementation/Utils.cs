using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StanskinsImplementation
{
    public class FakeComparable : IComparable<FakeComparable>
    {
        public int CompareTo(FakeComparable other)
        {
            return 0;
        }
    }
    public static class Utils
    {
        public static bool CompareDictionary(Dictionary<string, object> x, Dictionary<string, object> y)
        {
            if (x == null && y == null)
                return true;

            if (x != null && y == null)
                return false;

            if (x == null && y != null)
                return true;

            if (x.Count != y.Count)
                return false;

            //Order of keys doesn't matter
            foreach (KeyValuePair<string, object> item in x)
            {
                if (!y.ContainsKey(item.Key))
                    return false;

                var val = y[item.Key];
                if (item.Value == null && val == null)
                    return true;

                if (item.Value == null && val != null)
                    return false;
                if (item.Value != null && val == null)
                    return false;

                //Hack: ToString() used to force value comp.
                // item.Value should be a value type (not a ref type)
                if (!item.Value.ToString().Equals(val.ToString()))
                    return false;
            }

            return true;
        }

        //Extension method used to convert List<Dictionary<string, string>> to List<Dictionary<string,object>>
        public static List<Dictionary<string, object>> ToDictionaryStringObject(this List<Dictionary<string, string>> source)
        {
            if (source == null)
                return null;

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach(var item in source)
            {
                result.Add(item.ToDictionary(k => k.Key, v => (object)v.Value));
            }
            return result;
        }
    }
}
