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
    public class SimpleRow : IRow
    {
        public Dictionary<string, object> Values { get; set; }
    }
}
