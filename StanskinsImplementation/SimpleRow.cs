using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StankinsInterfaces
{
    
    public class SimpleRow : IRow, IEqualityComparer<SimpleRow>
    {
        public SimpleRow()
        {
            Values = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Values { get; set; }

        public bool Equals(SimpleRow x, SimpleRow y) //I'm expecting Equals method to be static
        {

            if (x == null && y == null)
                return true;

            if (x != null && y == null)
                return false;

            if (x == null && y != null)
                return true;

            if (x.Values?.Count != y.Values?.Count)
                return false;
            //Order of items doesn't matter
            foreach (KeyValuePair<string, object> item in x.Values)
            {
                if (!y.Values.ContainsKey(item.Key))
                    return false;
                var val = y.Values[item.Key];
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
        public override int GetHashCode()
        {
            var vKeys = Values.Keys.Sum(it => it.GetHashCode());
            var vValues = Values.Values.Sum(it => (it == null) ? 0 : it.GetHashCode());
            return vKeys + vValues;
        }

        public int GetHashCode(SimpleRow obj)
        {
            if (obj == null)
                return 0;

            return obj.GetHashCode();
        }
    }
}
