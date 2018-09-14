using Newtonsoft.Json;
using System.Collections.Generic;

namespace StanskinsImplementation
{
    [JsonDictionary]
    public class SimpleTree<K, V> : Dictionary<K, SimpleTree<K, V>>
    {
        public V Value { get; set; }
    }
}
