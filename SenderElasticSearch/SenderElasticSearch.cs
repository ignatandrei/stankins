using Nest;
using StankinsInterfaces;
using System;
using System.Threading.Tasks;

namespace SenderElasticSearch
{
    public class SenderToElasticSearch : ISend
    {
        public string Uri { get; set; }
        public string IndexName { get; set; }
        //Hack: added TypeName and Id properties
        //Hack: next version should define a new interface IRowTyped (:IRow) having two more props (TypeName and Id)
        public string TypeName { get; set; }
        public string Id { get; set; }

        public SenderToElasticSearch(string uri, string indexName, string typeName, string id)
        {
            this.Uri = uri;
            this.IndexName = indexName;
            this.TypeName = typeName;
            this.Id = id;
        }

        public IRow[] valuesToBeSent { set; get; }

        public async Task Send()
        {
            var settings = new ConnectionSettings(new Uri(this.Uri));
            var client = new ElasticClient(settings);

            foreach(IRow item in valuesToBeSent)
            {
                IndexRequest<object> request;

                if (string.IsNullOrWhiteSpace(this.Id))
                {
                    request = new IndexRequest<object>(this.IndexName, this.TypeName);
                }
                else
                {
                    string idValue = item.Values[this.Id].ToString(); //TODO: better solution ? (API Nest.Id string/long/Document)
                    request = new IndexRequest<object>(this.IndexName, this.TypeName, idValue);
                }

                request.Document = item.Values;
                IIndexResponse resp = await client.IndexAsync(request);
                if(!resp.ApiCall.Success)
                {
                    throw new Exception(resp.ApiCall.DebugInformation);
                }
            }
        }

        
    }
}
