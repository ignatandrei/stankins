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
using SenderToFile;
using SenderFilter;
using System.Diagnostics;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderFilter
    {
        //SQL Server general settings
        const string connectionString = @"Server=(local)\SQL2016;Database=tempdb;Trusted_Connection=True;";
        const CommandType commandType = CommandType.StoredProcedure;
        //SMTP general settings
        const string from = "666def2ad8-a3dd31@inbox.mailtrap.io";
        const string to = "bogdan@localhost.com";
        const string smtpServer = "smtp.mailtrap.io"; //https://mailtrap.io/ User:"bsahlean@gmail.com" Password:"SDJSf54c9c12fOEIRNYfhdsffdhFTBD5b05f43a99KLXCP" without double quotes; Free plan = maximum 50 emails allowed
        const int smtpPort = 2525;
        const string subject = "Send filtered data by email";
        const string body = "";
        const bool requiresAuthentication = true;
        const string user = "0b7c108f1bd651";
        const string password = "28fe1e0e2be31f";
        const string pop3Server = "mailtrap.io";
        const int pop3Port = 9950; // 1100 or 9950

        [TestMethod]
        public async Task TestSenderWithFilterSimpleNonEmptyData()
        {
            var dir = AppContext.BaseDirectory;

            #region arange
            string filename = Path.Combine(dir, "b.csv");
            if (File.Exists(filename))
                File.Delete(filename);

            var rows = new List<IRow>();
            int nrRows = 3;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["PersonID"] = i,
                        ["FirstName"] = "John" + i,
                        ["LastName"] = "Doe" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }
            #endregion

            #region act
            FilterComparable fltPerson = new FilterComparableGreaterOrEqual(typeof(Int32), 1, "PersonID");
            ISend csvExport = new Sender_CSV(filename);
            ISend filteredCsvExport = new SenderWithFilterComparable(fltPerson, csvExport);
            filteredCsvExport.valuesToBeSent = rows.ToArray();

            await filteredCsvExport.Send();
            #endregion

            #region assert
            Assert.IsTrue(File.Exists(filename), $"file {filename} must exists in export csv");
            var lines = File.ReadAllLines(filename);
            Assert.AreEqual(nrRows, lines.Length); // nrRows = nrRows + 1(header) - 1 (PersonID = 0)
            var lineNames = lines[0].Split(',');
            Assert.AreEqual(lineNames[0], "PersonID");
            Assert.AreEqual(lineNames[1], "FirstName");
            Assert.AreEqual(lineNames[2], "LastName");
            #endregion
        }

        [TestMethod]
        public async Task TestSenderWithFilterSimpleEmptyData()
        {
            var dir = AppContext.BaseDirectory;

            #region arange
            string filename = Path.Combine(dir, "b.csv");
            if (File.Exists(filename))
                File.Delete(filename);

            var rows = new List<IRow>();
            int nrRows = 3;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["PersonID"] = i,
                        ["FirstName"] = "John" + i,
                        ["LastName"] = "Doe" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }
            #endregion

            #region act
            FilterComparable fltPerson = new FilterComparableGreaterOrEqual(typeof(Int32), 10, "PersonID");
            ISend csvExport = new Sender_CSV(filename);
            ISend filteredCsvExport = new SenderWithFilterComparable(fltPerson, csvExport);
            filteredCsvExport.valuesToBeSent = rows.ToArray();

            await filteredCsvExport.Send();
            #endregion

            #region assert
            Assert.IsFalse(File.Exists(filename), $"file {filename} must not exists because default behaviour is to not execute real sender when filter doesn't produce at least one row");
            #endregion
        }

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSenderWithFilterSQL2SQL2SMTP()
        {
            #region arrange
            string commandText1 = "dbo.TestReiceverDBExecuteStoredProcedureNoParam2"; // Receiver SP (Source)
            string fileNameSerilizeLastRow = string.Empty;
            string parameters1 = string.Empty;

            string commandText2 = "dbo.TestSenderDBExecuteStoredProcedureWithParams2"; // Sender SP (Destination)
            string parameters2 = "@p1=PersonID;@p2=FirstName;@p3=LastName";

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

            //Job
            var rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText1, fileNameSerilizeLastRow, parameters1);
            var sndAll = new SenderToDBStmtSqlServer(connectionString, commandType, commandText2, parameters2);
            //It's not the real sender being used by sndSMTPFiltered
            var sndSMTP = new SenderToSMTP(from, to, string.Empty, string.Empty, subject, string.Empty, false, smtpServer, smtpPort, false, requiresAuthentication, user, password);
            //Real sender
            FilterComparable fltPerson = new FilterComparableGreaterOrEqual(typeof(Int32), 1, "PersonID");
            ISend sndSMTPFiltered = new SenderWithFilterComparable(fltPerson, sndSMTP);

            var job = new SimpleJob();
            job.Receivers.Add(0, rcvr);
            job.Senders.Add(0, sndAll);
            job.Senders.Add(1, sndSMTPFiltered);

            /*
            var job = new SimpleJob();
            job.Receivers.Add(0, rcvr);
            job.Senders.Add(0, sndSMTP);

            var j = job.SerializeMe();
            File.WriteAllText("j.json", j);
            Process.Start("Notepad.exe", "j.json");
            */
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
