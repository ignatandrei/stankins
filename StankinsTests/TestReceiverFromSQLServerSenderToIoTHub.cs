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
            string iotHubUri = "AzBogdanStankinsIoTHub.azure-devices.net";
            string deviceId = "DeviceTest01-ACD3688D";
            string deviceKey = "oXJiz/W9Ta4dNM6s6FTmm2K14RxuFjbrKHXbxteYoRs=";
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
            //Assert Receiver IoT Hub settings
            string iotHubConnectionStringEventHubCompatible = "Endpoint=sb://iothub-ns-azbogdanst-208965-a24331514f.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=pPQtX7pSbtNM1cUngtgsdRJIopXGF/jfHZPRVtlcebg=";
            string iotHubMessageEntityEventHubCompatible = "azbogdanstankinsiothub";
            string fileNameLastRow = "TestSimpleJobReceiverFromSQLServer2SenderToIoTHub_LastOffset.json";

            var rcv = new ReceiverFromAzureIoTHub(iotHubConnectionStringEventHubCompatible, iotHubMessageEntityEventHubCompatible, fileNameLastRow, messageType, -1);
            await rcv.LoadData();
            bool hasFirstRow = false;
            bool hasSecondRow = false;
            foreach(var row in rcv.valuesRead)
            {
                if(row.Values.ContainsKey("PersonID") && row.Values.ContainsKey("FirstName") && row.Values.ContainsKey("LastName"))
                {
                    hasFirstRow = ((long)row.Values["PersonID"] == 10 && (string)row.Values["FirstName"] == "John 00" && (string)row.Values["LastName"] == "Doe 00") ? true : hasFirstRow;
                    hasSecondRow = ((long)row.Values["PersonID"] == 11 && (string)row.Values["FirstName"] == "John 01" && (string)row.Values["LastName"] == "Doe 01") ? true : hasSecondRow;
                }
            }
            Assert.IsTrue(hasFirstRow);
            Assert.IsTrue(hasSecondRow);
            #endregion
        }
    }
}
