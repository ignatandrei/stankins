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
    public class CtorDictionaryGeneric<T> : Dictionary<string, T>
    {
        public CtorDictionaryGeneric() : base(new CompareStrings())
        {

        }
        public CtorDictionaryGeneric(IDictionary<string, T> data) : base(data, new CompareStrings())
        {

        }
        public CtorDictionaryGeneric<T>  AddValue(string key, T val)
        {
            this.Add(key,val);
            return this;
        }
    }
    public class CtorDictionary: CtorDictionaryGeneric<object>
    {
        public CtorDictionary():base()
        {

        }
        public CtorDictionary(IDictionary<string,object> data):base(data)
        {

        }
        public CtorDictionary  AddMyValue(string key, object val)
        {
            if (this.ContainsKey(key))
            {
                Console.WriteLine($"key exists {key} with {val}");
                this.Remove(key);
            }
            this.Add(key,val);
            return this;
        }
    }
}
