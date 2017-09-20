using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDBStmtSqlServer;
using System.Threading.Tasks;
using Nest;
using SenderElasticSearch;
using StankinsInterfaces;
using StanskinsImplementation;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using SenderAzureIoTHub;
using ReceiverAzureIoTHub;
using System.Diagnostics;
using SenderDBStmtSqlServer;
using Moq;
using Shouldly;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverSQLServer2SenderIoTHub
    {
        const CommandType commandType = CommandType.StoredProcedure;

        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }

        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSimpleJobReceiverFromSQLServer2SenderToIoTHub()
        {
            #region arrange
            string connectionString = GetSqlServerConnectionString();
            string commandText1 = "dbo.TestReiceverDBExecuteStoredProcedureNoParam3"; // Receiver SP (Source)
            string fileNameSerilizeLastRow = string.Empty;
            string parameters1 = string.Empty;

            //Receiver
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam3') IS NOT NULL DROP TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam3; CREATE TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam3 (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50)); INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam3 VALUES (10, 'John 00', 'Doe 00'), (11, 'John 01', 'Doe 01');";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureNoParam3') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam3;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam3 AS SELECT * FROM dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam3;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            var rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText1, fileNameSerilizeLastRow, parameters1);

            //Sender
            string iotHubUri = "a";
            string deviceId = "b";
            string deviceKey = "c";
            string messageType = "UnitTest";

            var snd = new SenderToAzureIoTHub(iotHubUri, deviceId, deviceKey, messageType);
            //Job
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, rcvr);
            job.Senders.Add(0, snd);
            #endregion

            #region act
            await job.Execute();
            //var j = job.SerializeMe();
            //File.WriteAllText(@"e:\j.json", j);
            #endregion

            #region assert
            rcvr.valuesRead.Length.ShouldBe(2);
            //Assert Receiver IoT Hub settings
            string iotHubConnectionStringEventHubCompatible = "Endpoint=sb://iothub-ns-azbogdanst-208965-a24331514f.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=pPQtX7pSbtNM1cUngtgsdRJIopXGF/jfHZPRVtlcebg=";
            string iotHubMessageEntityEventHubCompatible = "azbogdanstankinsiothub";
            string fileNameLastRow = "TestSimpleJobReceiverFromSQLServer2SenderToIoTHub_LastOffset.json";

            var rcv = new ReceiverFromAzureIoTHub(iotHubConnectionStringEventHubCompatible, iotHubMessageEntityEventHubCompatible, fileNameLastRow, messageType, -1);
            await rcv.LoadData();
            bool hasFirstRow = false;
            bool hasSecondRow = false;
            rcv.valuesRead.ShouldNotBeNull();
            rcv.valuesRead.Length.ShouldBeGreaterThanOrEqualTo(2);
            foreach(var row in rcv.valuesRead)
            {
                if(row.Values.ContainsKey("PersonID") && row.Values.ContainsKey("FirstName") && row.Values.ContainsKey("LastName"))
                {
                    hasFirstRow = ((long)row.Values["PersonID"] == 10 && (string)row.Values["FirstName"] == "John 00" && (string)row.Values["LastName"] == "Doe 00") ? true : hasFirstRow;
                    hasSecondRow = ((long)row.Values["PersonID"] == 11 && (string)row.Values["FirstName"] == "John 01" && (string)row.Values["LastName"] == "Doe 01") ? true : hasSecondRow;
                }
            }
            Assert.IsTrue(hasFirstRow,"must have first row");
            Assert.IsTrue(hasSecondRow,"must have second row");
            #endregion
        }

        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSimpleJobReceiverFromIoTHub2SenderToSQLServer()
        {
            #region arrange
            //Send test message
            string iotHubUri = "a";
            string deviceId = "b";
            string deviceKey = "c";
            string messageType = "UnitTestSimpleJobReceiverFromIoTHub2SenderToSQLServer" + DateTime.Now.ToString();

            var sndToIoTHub = new SenderToAzureIoTHub(iotHubUri, deviceId, deviceKey, messageType);
            var m = new Mock<IRow>();
            var rows = new List<IRow>();
            int nrRows = 2;

            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();
                row.SetupProperty
                (
                    obj => obj.Values,
                    new Dictionary<string, object>()
                    {
                        ["PersonID"] = i,
                        ["FirstName"] = "John " + i,
                        ["LastName"] = "Doe " + i
                    }
                );

                rows.Add(row.Object);
            }
            sndToIoTHub.valuesToBeSent = rows.ToArray();
            await sndToIoTHub.Send();
            //End of Send test message

            //Receiver settings
            string iotHubConnectionStringEventHubCompatible = "a";
            string iotHubMessageEntityEventHubCompatible = "b";
            string fileNameLastRow = "TestReceiveAzureIoTHubSimple_LastRow.json";

            //Receiver
            string connectionString = GetSqlServerConnectionString();
            string commandText2 = "dbo.TestReiceverDBExecuteStoredProcedureNoParam4"; // Sender SP (Destination)
            string parameters2 = "@p1=PersonID;@p2=FirstName;@p3=LastName";

            //Source
            if (File.Exists(fileNameLastRow))
            {
                File.Delete(fileNameLastRow);
            }

            var rcv = new ReceiverFromAzureIoTHub(iotHubConnectionStringEventHubCompatible, iotHubMessageEntityEventHubCompatible, fileNameLastRow, messageType, -4);

            //Destination
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam4') IS NOT NULL DROP TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam4; CREATE TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam4 (PersonID INT NOT NULL /*PRIMARY KEY*/, FirstName VARCHAR(50), LastName VARCHAR(50));";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureNoParam4') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam4;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam4 (@p1 INT, @p2 VARCHAR(50), @p3 VARCHAR(50)) AS INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam4 (PersonID, FirstName, LastName) VALUES (@p1, @p2, @p3)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            var snd = new SenderToDBStmtSqlServer(connectionString, commandType, commandText2, parameters2);

            //Job
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, rcv);
            job.Senders.Add(0, snd);
            //var j = job.SerializeMe();
            //File.WriteAllText(@"E:\j2.json", j);
            #endregion

            #region act
            await job.Execute();
            #endregion

            #region assert
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) AS Cnt FROM TestingTestReiceverDBExecuteStoredProcedureNoParam4;";
                    var cnt = (int)await cmd.ExecuteScalarAsync();
                    Assert.IsTrue(cnt >= 2); // It requires a sender in arrange region
                }
            }
            #endregion
        }
    }
}
