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
using System.IO;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverDBStmtSqlServer
    {
        const string connectionString = @"Server=(local)\SQL2016;Database=tempdb;Trusted_Connection=True;";
        const CommandType commandType = CommandType.StoredProcedure;
        
        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestReiceverDBExecuteStoredProcedureNoParams()
        {
            #region arange
            string commandText = "dbo.TestReiceverDBExecuteStoredProcedureNoParams";
            const string fileNameSerilizeLastRow = "TestReiceverDBExecuteStoredProcedureNoParams_LastRow.txt";

            if (File.Exists(fileNameSerilizeLastRow))
            {
                File.Delete(fileNameSerilizeLastRow);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureNoParams') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParams;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParams AS SELECT 1 AS PersonID, 'John' AS FirstName , 'Doe' AS LastName UNION ALL SELECT 11, 'Joanna', 'Doe' ORDER BY PersonID";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            
            ReceiverStmtSqlServer rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText, fileNameSerilizeLastRow);
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
            Assert.AreEqual("Joanna", results[1].Values["FirstName"]);
            Assert.AreEqual("Doe", results[1].Values["LastName"]);
            //lastRow ?
            SerializeDataOnFile sdf = new SerializeDataOnFile(fileNameSerilizeLastRow);
            Dictionary<string, object> lastRowRead = sdf.GetDictionary();
            //lastRow Count ? 
            Assert.AreEqual(3, lastRowRead.Count);
            //lastRow data ?
            Assert.AreEqual(11, (long)lastRowRead["PersonID"]);
            Assert.AreEqual("Joanna", lastRowRead["FirstName"]);
            Assert.AreEqual("Doe", lastRowRead["LastName"]);
            #endregion
        }

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestReiceverDBExecuteStoredProcedureWithParams()
        {
            //First call
            #region arange
            string commandText = "dbo.TestReiceverDBExecuteStoredProcedureWithParam";
            const string fileNameSerilizeLastRow = "TestReiceverDBExecuteStoredProcedureWithParam_LastRow.txt";
            string parameters = "@pid=PersonID;@p2=FirstName";

            if (File.Exists(fileNameSerilizeLastRow))
            {
                File.Delete(fileNameSerilizeLastRow);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam') IS NOT NULL DROP TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam; CREATE TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50)); INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam VALUES (1, 'John', 'Doe'), (11, 'Joanna', 'Doe');";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureWithParam') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureWithParam;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureWithParam (@pid INT, @p2 VARCHAR(50)) AS SELECT * FROM dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam x WHERE x.PersonID > ISNULL(@pid,0) ORDER BY PersonID";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            ReceiverStmtSqlServer rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText, fileNameSerilizeLastRow, parameters);
            #endregion

            #region act
            await rcvr.LoadData();
            #endregion

            #region assert
            var results = rcvr.valuesRead;
            //Same number of rows ?
            Assert.AreEqual(2, results.Length);
            //Same data ?
            Assert.AreEqual(1, results[0].Values["PersonID"]);
            Assert.AreEqual("John", results[0].Values["FirstName"]);
            Assert.AreEqual("Doe", results[0].Values["LastName"]);
            Assert.AreEqual(11, results[1].Values["PersonID"]);
            Assert.AreEqual("Joanna", results[1].Values["FirstName"]);
            Assert.AreEqual("Doe", results[1].Values["LastName"]);
            //lastRow ?
            SerializeDataOnFile sdf = new SerializeDataOnFile(fileNameSerilizeLastRow);
            Dictionary<string, object> lastRowRead = sdf.GetDictionary();
            //lastRow data ?
            Assert.AreEqual(11, (long)lastRowRead["PersonID"]);
            Assert.AreEqual("Joanna", lastRowRead["FirstName"]);
            Assert.AreEqual("Doe", lastRowRead["LastName"]);
            #endregion

            //Second call (we just calling twice the same stored procedure)
            #region arrage
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureWithParam VALUES (111, 'Ion', 'Ion'),(1111, 'Ioan', 'Ioan');";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            #endregion

            #region act
            await rcvr.LoadData();
            #endregion

            #region assert
            results = rcvr.valuesRead;
            //Same number of rows ?
            Assert.AreEqual(2, results.Length);
            //Same data ?
            Assert.AreEqual(111, results[0].Values["PersonID"]);
            Assert.AreEqual("Ion", results[0].Values["FirstName"]);
            Assert.AreEqual("Ion", results[0].Values["LastName"]);
            Assert.AreEqual(1111, results[1].Values["PersonID"]);
            Assert.AreEqual("Ioan", results[1].Values["FirstName"]);
            Assert.AreEqual("Ioan", results[1].Values["LastName"]);
            //lastRow ?
            sdf = new SerializeDataOnFile(fileNameSerilizeLastRow);
            lastRowRead = sdf.GetDictionary();
            //lastRow data ?
            Assert.AreEqual(1111, (long)lastRowRead["PersonID"]);
            Assert.AreEqual("Ioan", lastRowRead["FirstName"]);
            Assert.AreEqual("Ioan", lastRowRead["LastName"]);
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
            const string fileNameSerilizeLastRow = "TestExecStoredProcedure2.txt";

            #region arange
            //Arange receiver
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedure2') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure2;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedure2 AS SELECT 1 AS PersonID, 'John' AS FirstName , 'Doe' AS LastName UNION ALL SELECT 11, 'Joanna', 'Doe' ORDER BY PersonID";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            ReceiverStmtSqlServer rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText, fileNameSerilizeLastRow);

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
            Dictionary<string, object>[] valuesReadFromESOrdered = (new List<Dictionary<string, string>>(responseAll.Documents)).OrderBy(ord => ord[id]).ToList<Dictionary<string, string>>().ToDictionaryStringObject().ToArray();
            Assert.IsTrue(Utils.CompareDictionary(rcvr.valuesRead[0].Values, valuesReadFromESOrdered[0]));
            Assert.IsTrue(Utils.CompareDictionary(rcvr.valuesRead[1].Values, valuesReadFromESOrdered[1]));
            #endregion
        }
    }
} 
