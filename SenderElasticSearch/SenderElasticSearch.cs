using Nest;
using StankinsInterfaces;
using System;
using System.Threading.Tasks;

namespace SenderElasticSearch
{
    /// <summary>
    /// Send data to Elasticsearch instance.
    /// </summary>
    public class SenderToElasticSearch : ISend
    {
        /// <summary>
        /// Name of sender instance.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The URI of Elasticsearch instance. Example: <code>http://localhost:9200</code>.
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// The name of Elasticsearch index. Example: <code>index-active-slow-query</code>.
        /// </summary>
        public string IndexName { get; set; }
        //Hack: added TypeName and Id properties
        //Hack: next version should define a new interface IRowTyped (:IRow) having two more props (TypeName and Id)
        /// <summary>
        /// The name of Elasticsearch type. Example: <code>active-slow-query</code>.
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// The name of Elasticsearch property having the role of identifier. See also <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-id-field.html">_id field</see>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the ReceiverCSVFile class.
        /// </summary>
        /// <param name="uri">See <see cref="Uri"/> property.</param>
        /// <param name="indexName">See <see cref="IndexName"/> property.</param>
        /// <param name="typeName">See <see cref="TypeName"/> property.</param>
        /// <param name="id">See <see cref="Id"/> property.</param>
        public SenderToElasticSearch(string uri, string indexName, string typeName, string id)
        {
            this.Uri = uri;
            this.IndexName = indexName;
            this.TypeName = typeName;
            this.Id = id;
        }

        /// <summary>
        /// Get or set the IRow object used to store values to be sent.
        /// </summary>
        public IRow[] valuesToBeSent { set; get; }

        /// <summary>
        /// Executes the sender reading data from valuesToBeSent.
        /// </summary>
        /// <returns>Task</returns>
        /// <example>To copy data from SQL Server to Elasticsearch following *.json configuration file can be used:
        /// <code title="SQLServerToElasticsearch.json">
        /// <![CDATA[{
        ///   "$type": "StanskinsImplementation.SimpleJob, StanskinsImplementation",
        ///   "Receivers": {
        ///     "$type": "StankinsInterfaces.OrderedList`1[[StankinsInterfaces.IReceive, StankinsInterfaces]], StankinsInterfaces",
        ///     "0": {
        ///       "$type": "ReiceverDBStmtSqlServer.ReceiverStmtSqlServer, ReiceverDBStmtSqlServer",
        ///       "ConnectionString": "Server=(local)\\SQL2016;Database=IronSQLDBA;Trusted_Connection=True;",
        ///       "CommandType": 4,
        ///       "CommandText": "dbo.active_slow_query_select",
        ///       "FileNameSerializeLastRow": "IronSQLDBA_active_slow_query_select_last_row.json",
        ///       "ParametersMappings": "@original_id=original_id"
        ///     }
        ///   },
        ///   "FiltersAndTransformers": {
        ///     "$type": "StankinsInterfaces.OrderedList`1[[StankinsInterfaces.IFilterTransformer, StankinsInterfaces]], StankinsInterfaces"
        ///   },
        ///   "Senders": {
        ///     "$type": "StankinsInterfaces.OrderedList`1[[StankinsInterfaces.ISend, StankinsInterfaces]], StankinsInterfaces",
        ///     "0": {
        ///       "$type": "SenderElasticSearch.SenderToElasticSearch, SenderElasticSearch",
        ///       "Uri": "http://localhost:9200",
        ///       "IndexName": "ironsqldba-index-active-slow-query",
        ///       "TypeName": "active-slow-query",
        ///       "Id": "id"
        ///     }	
        ///   }
        /// }]]>
        /// </code>
        /// </example>
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
