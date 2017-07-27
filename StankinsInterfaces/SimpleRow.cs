using System;
using System.Collections.Generic;
using System.Text;

namespace StankinsInterfaces
{
    /*
    Used by TestSendElasticSearch.TestSendElasticSearchData. It replace mock objects which are trowing following exception
    Message: Test method StankinsTests.TestSenderElasticSearch.TestSendElasticSearchData threw exception: 
    Elasticsearch.Net.UnexpectedElasticsearchClientException: Self referencing loop detected for property 'object' with type 'Castle.Proxies.ObjectProxy'. Path 'mock'. ---> Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property 'object' with type 'Castle.Proxies.ObjectProxy'. Path 'mock'.
    */
    public class SimpleRow : IRow, IEqualityComparer<SimpleRow>
    {
        public Dictionary<string, object> Values { get; set; }

        public bool Equals(SimpleRow x, SimpleRow y) //I'm expecting Equals method to be static
        {
            bool result = false;
            if ( x != null && y != null && x.Values != null && y.Values != null && x.Values.Count == y.Values.Count)
            {
                result = true;
                //Order of items doesn't matter
                foreach (KeyValuePair<string, object> item in x.Values)
                {
                    //Hack: ToString() used to force value comp.
                    if ( !y.Values.ContainsKey(item.Key) || y.Values.ContainsKey(item.Key) && !item.Value.ToString().Equals(y.Values[item.Key].ToString())) // item.Value should be a value type (not a ref type)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else if( x == null && y == null || x != null && y != null && x.Values == null && y.Values == null )
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public int GetHashCode(SimpleRow obj)
        {
            throw new NotImplementedException();
        }
    }
}
