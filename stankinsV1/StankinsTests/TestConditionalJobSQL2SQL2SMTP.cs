using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using StankinsInterfaces;
using Moq;
using SenderDBStmtSqlServer;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ReceiverDBStmtSqlServer;
using StanskinsImplementation;
using SenderSMTP;
using Transformers;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Configuration;

namespace StankinsTests
{
    [TestClass]
    public class TestConditionalJobSQL2SQL2SMTP
    {
        //SQL Server general settings
        const CommandType commandType = CommandType.StoredProcedure;
        //SMTP general settings
        const string from = "666def2ad8-a3dd31@inbox.mailtrap.io";
        const string to = "bogdan@localhost.com";
        const string smtpServer = "smtp.mailtrap.io"; //https://mailtrap.io/ 
        const int smtpPort = 2525;
        const string subject = "Send data by email";
        const string body = "Person details:\n";
        const bool requiresAuthentication = true;
        const string user = "0b7c108f1bd651";
        const string password = "28fe1e0e2be31f";
        const string pop3Server = "mailtrap.io";
        const int pop3Port = 9950; // 1100 or 9950

        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }

        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestConditionalJobSQLServer2SQLServerAndFilter2SMTP()
        {
            #region arrange
            string connectionString = GetSqlServerConnectionString();
            string commandText1 = "dbo.TestReiceverDBExecuteStoredProcedureNoParam2"; // Receiver SP (Source)
            string fileNameSerilizeLastRow = string.Empty;
            string parameters1 = string.Empty;

            string commandText2 = "dbo.TestSenderDBExecuteStoredProcedureWithParams2"; // Sender SP (Destination)
            string parameters2 = "@p1=PersonID;@p2=FirstName;@p3=LastName";

            //Receiver: SQL Server Source
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2') IS NOT NULL DROP TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2; CREATE TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2 (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50)); INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2 VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1'), (2, 'John 2', 'Doe 2');";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureNoParam2') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam2;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam2 AS SELECT * FROM dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            var rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText1, fileNameSerilizeLastRow, parameters1);

            //Sender: SQL Server Destination
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestSenderDBExecuteStoredProcedureWithParams2') IS NOT NULL DROP TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams2; CREATE TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams2 (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50));";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestSenderDBExecuteStoredProcedureWithParams2') IS NOT NULL DROP PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams2;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams2 (@p1 INT, @p2 VARCHAR(50), @p3 VARCHAR(50)) AS INSERT dbo.TestingTestSenderDBExecuteStoredProcedureWithParams2 (PersonID, FirstName, LastName) VALUES (@p1, @p2, @p3)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            var sndAll = new SenderToDBStmtSqlServer(connectionString, commandType, commandText2, parameters2);

            //Filter
            var fltPersCateg1 = new FilterComparableGreaterOrEqual(typeof(int), 1, "PersonID");

            //Sender: SMTP Alert Configuration
            var sndPersCateg1 = new SenderToSMTP(from, to, string.Empty, string.Empty, subject, string.Empty, false, smtpServer, smtpPort, false, requiresAuthentication, user, password);

            //Sender: SMTP Clean
            using (var client = new Pop3Client())
            {
                client.Connect(pop3Server, pop3Port, false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(user, password);

                for (int i = 0; i < client.Count; i++)
                {
                    var message = client.GetMessage(i);
                    if (message.Subject == subject)
                    {
                        client.DeleteMessage(i);
                    }
                }
                client.Disconnect(true);
            }


            //Job
            var job = new SimpleJobConditionalTransformers();
            job.Receivers.Add(0, rcvr);
            job.AddSender(sndAll);
            job.Add(fltPersCateg1, sndPersCateg1);
            #endregion

            #region act
            await job.Execute();
            #endregion

            #region assert
            //Check destination table
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) AS Cnt FROM TestingTestSenderDBExecuteStoredProcedureWithParams2 tbl FULL JOIN (VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1'), (2, 'John 2', 'Doe 2')) chk(PersonID, FirstName, LastName) ON tbl.PersonID = chk.PersonID AND tbl.FirstName = chk.FirstName AND tbl.LastName = chk.LastName WHERE EXISTS(SELECT tbl.PersonID EXCEPT SELECT chk.PersonID) OR EXISTS(SELECT tbl.FirstName EXCEPT SELECT chk.FirstName) OR EXISTS(SELECT tbl.LastName EXCEPT SELECT chk.LastName);";
                    var cnt = (int)await cmd.ExecuteScalarAsync();
                    Assert.AreEqual(0, cnt);
                }
            }
            //Check email
            //Read message and check Body (plain text)
            int numOfEmailFound = 0;
            using (var client = new Pop3Client())
            {
                client.Connect(pop3Server, pop3Port, false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(user, password);

                for (int i = 0; i < client.Count; i++)
                {
                    var message = client.GetMessage(i);
                    if (message.Subject == subject)
                    {
                        numOfEmailFound++;
                    }
                }
                client.Disconnect(true);
            }
            Assert.AreEqual(1, numOfEmailFound);
            #endregion
        }
    }
}
