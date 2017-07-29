using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nest;
using SenderElasticSearch;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderElasticSearch
    {
        //public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSendElasticSearchData()
        {
            const string url = "http://localhost:9200";
            const string indexName = "ixtestsenderelasticsearch";
            const string typeName = "Person";
            const string id = "PersonID";

            #region arange
            //Clean ES test index
            var settings = new ConnectionSettings(new Uri(url));
            var client = new ElasticClient(settings);

            if (client.IndexExists(indexName).Exists)
            {
                 client.DeleteIndex(indexName);
            }

            //Prepare source data: 4 rows {ID, FirstName, LastName} + 5th row {ID, FirstName}
            var rows = new List<IRow>();
            int nrRows = 4;
            for (int i = 0; i < nrRows; i++)
            {
                var row = new SimpleRow();
                row.Values = new Dictionary<string, object>();
                row.Values["PersonID"] = i;
                row.Values["FirstName"] = "John " + i;
                row.Values["LastName"] = "Doe " + i;

                rows.Add(row);
            }
            //5th row (no LastName)
            {
                var row = new SimpleRow();
                var i = 4;

                row.Values = new Dictionary<string, object>();
                row.Values["PersonID"] = i;
                row.Values["FirstName"] = "John " + i;
                row.Values["LastName"] = "Doe " + i;

                rows.Add(row);
            }
            #endregion

            #region act
            ISend esExport = new SenderToElasticSearch(url, indexName, typeName, id);
            esExport.valuesToBeSent = rows.ToArray();
            await esExport.Send();
            await Task.Delay(5000); //Missing of await keyword is indentional (Thread.Sleep(5000)). 5s shoudl be more than enough to insert 5 documents.
            #endregion

            #region assert
            //Same count ?
            var responseAll = client.Search<Dictionary<string, string>>(s => s.Size(10).Index(indexName).Type(typeName));
            int countAll = responseAll.Documents.Count;
            Assert.AreEqual(countAll, 5, $"Inserted documents: 5, Read documents {countAll}");

            //Same values ? (assuption: [1] both count are equals,  [2] rows is already sorted by id)
            var itemsReadFromESOrdered = (new List<Dictionary<string, string>>(responseAll.Documents)).OrderBy(ord => ord[id]).ToList<Dictionary<string, string>>();
            for (int i = 0; i < countAll; i++)
            {
                SimpleRow r1 = (SimpleRow)rows[i];
                SimpleRow r2 = new SimpleRow() { Values = itemsReadFromESOrdered[i].ToDictionary(k => k.Key, v => (object)v.Value) };
                Assert.IsTrue(r1.Equals(r1, r2));
            }
            #endregion
        }
    }
}