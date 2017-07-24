using Nest;
using StankinsInterfaces;
using System;
using System.Threading.Tasks;

namespace SenderElasticSearch
{
    public class SenderSearch : ISend
    {
        public string URL { get; set; }
        public string IndexName { get; set; }
        
        public SenderSearch(string url, string indexName)
        {
            this.URL = url;
            this.IndexName = indexName;
        }

        public IRow[] valuesToBeSent { set; get; }

        public async Task Send()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("ix004");
            var client = new ElasticClient(settings);
            foreach(var item in valuesToBeSent)
            {
                await client.IndexAsync(item.Values);
            }
        }

        
    }
}
