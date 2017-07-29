using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReiceverDBStmtSqlServer;
using System.Threading.Tasks;
using Nest;
using SenderElasticSearch;
using StankinsInterfaces;
using StanskinsImplementation;
using System.Collections.Generic;
using System.Linq;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverDBStmtSqlServer
    {
        const string connectionString = @"Server=(local)\SQL2016;Database=tempdb;Trusted_Connection=True;";
        const CommandType commandType = CommandType.StoredProcedure;

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestReiceverDBExecuteStoredProcedure()
        {
            #region arange
            string commandText = "dbo.TestReiceverDBExecuteStoredProcedure";

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "IF OBJECT_ID('tempdb.dbo.TestReiceverDBExecuteStoredProcedure') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure AS SELECT 1 AS PersonID, 'John' AS FirstName , 'Doe' AS LastName UNION ALL SELECT 11, 'Joahne', 'Doe' ORDER BY PersonID";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            
            ReceiverStmtSqlServer rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText);
            #endregion

            #region act
            await rcvr.LoadData();
            #endregion

            #region assert
            var results = rcvr.valuesRead;
            //Same number of rows ?
            Assert.AreEqual(2, results.Length);
            //Same data
            Assert.AreEqual(1, results[0].Values["PersonID"]);
            Assert.AreEqual("John", results[0].Values["FirstName"]);
            Assert.AreEqual("Doe", results[0].Values["LastName"]);
            Assert.AreEqual(11, results[1].Values["PersonID"]);
            Assert.AreEqual("Joahne", results[1].Values["FirstName"]);
            Assert.AreEqual("Doe", results[1].Values["LastName"]);
            #endregion
        }

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSimpleJobReceiverDBExecuteStoredProcedureToSenderElasticSearch()
        {
            const string commandText = "dbo.TestReiceverDBExecuteStoredProcedure2";
            const string url = "http://localhost:9200";
            const string indexName = "ixtestsenderelasticsearch2";
            const string typeName = "Person";
            const string id = "PersonID";

            #region arange
            //Arange receiver
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "IF OBJECT_ID('tempdb.dbo.TestReiceverDBExecuteStoredProcedure2') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure2;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure2 AS SELECT 1 AS PersonID, 'John' AS FirstName , 'Doe' AS LastName UNION ALL SELECT 11, 'Joahne', 'Doe' ORDER BY PersonID";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            ReceiverStmtSqlServer rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText);

            //Arange sender
            var settings = new ConnectionSettings(new Uri(url));
            var client = new ElasticClient(settings);

            if (client.IndexExists(indexName).Exists)
            {
                client.DeleteIndex(indexName);
            }

            //Arange simple job
            ISend snd = new SenderToElasticSearch(url, indexName, typeName, id);

            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, rcvr);
            job.Senders.Add(0, snd);
            #endregion

            #region act
            job.Execute().Wait();
            await Task.Delay(5000); //Missing of await keyword is indentional (Thread.Sleep(5000)). 5s shoudl be more than enough to insert 2 documents.
            #endregion

            #region assert
            //Same count ?
            var responseAll = client.Search<Dictionary<string, string>>(s => s.Size(10).Index(indexName).Type(typeName));
            int countAll = responseAll.Documents.Count;
            Assert.AreEqual(countAll, 2, $"Inserted documents: 2, Read documents {countAll}");

            //Same values ? (assuption: [1] both count are equals,  [2] rows is already sorted by id)
            var itemsReadFromESOrdered = (new List<Dictionary<string, string>>(responseAll.Documents)).OrderBy(ord => ord[id]).ToList<Dictionary<string, string>>();
            for (int i = 0; i < countAll; i++)
            {
                SimpleRow r1 = new SimpleRow() { Values = rcvr.valuesRead[i].Values };
                SimpleRow r2 = new SimpleRow() { Values = itemsReadFromESOrdered[i].ToDictionary(k => k.Key, v => (object)v.Value) };
                Assert.IsTrue(r1.Equals(r1, r2));
            }
            #endregion
        }
    }
} 
