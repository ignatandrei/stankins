using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StanskinsImplementation
{
    public class SerializeDataInMemory : ISerializeData
    {
        Dictionary<string, object> data;
        public SerializeDataInMemory()
        {
            data = new Dictionary<string, object>();
        }
        public object GetValue(string key)
        {
            if (!data.ContainsKey(key))
                return null;

            return data[key];
        }

        public void SetValue(string key, object value)
        {
            data[key] = value;
        }
    }
}
