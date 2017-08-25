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
using ReiceverDBStmtSqlServer;
using StanskinsImplementation;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderDBStmtSqlServer
    {
        string connectionString = @"Server=(local)\SQL2016;Database=tempdb;Trusted_Connection=True;";
        const CommandType commandType = CommandType.StoredProcedure;

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSenderDBExecuteStoredProcedureWithParams()
        {
            #region arrange
            string commandText = "dbo.TestSenderDBExecuteStoredProcedureWithParams";
            string parameters = "@p1=PersonID;@p2=FirstName;@p3=LastName";

            //Prepare source data: 2 rows {ID, FirstName, LastName}
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

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestSenderDBExecuteStoredProcedureWithParams') IS NOT NULL DROP TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams; CREATE TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50));";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestSenderDBExecuteStoredProcedureWithParams') IS NOT NULL DROP PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams (@p1 INT, @p2 VARCHAR(50), @p3 VARCHAR(50)) AS INSERT dbo.TestingTestSenderDBExecuteStoredProcedureWithParams (PersonID, FirstName, LastName) VALUES (@p1, @p2, @p3)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            var sender = new SenderToDBStmtSqlServer(connectionString, commandType, commandText, parameters);
            sender.valuesToBeSent = rows.ToArray();
            #endregion

            #region act
            await sender.Send();
            #endregion

            #region assert
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) AS Cnt FROM TestingTestSenderDBExecuteStoredProcedureWithParams tbl FULL JOIN (VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1')) chk(PersonID, FirstName, LastName) ON tbl.PersonID = chk.PersonID AND tbl.FirstName = chk.FirstName AND tbl.LastName = chk.LastName WHERE EXISTS(SELECT tbl.PersonID EXCEPT SELECT chk.PersonID) OR EXISTS(SELECT tbl.FirstName EXCEPT SELECT chk.FirstName) OR EXISTS(SELECT tbl.LastName EXCEPT SELECT chk.LastName);";
                    var cnt = (int)await cmd.ExecuteScalarAsync();
                    Assert.AreEqual(0, cnt);
                }
            }
            #endregion
        }

        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSimpleJobReceiverToSenderDBExecuteSqlServerStoredProcedures()
        {
            #region arrange
            string commandText1 = "dbo.TestReiceverDBExecuteStoredProcedureNoParam2"; // Receiver SP (Source)
            string fileNameSerilizeLastRow = string.Empty;
            string parameters1 = string.Empty; 

            string commandText2 = "dbo.TestSenderDBExecuteStoredProcedureWithParams2"; // Sender SP (Destination)
            string parameters2 = "@p1=PersonID;@p2=FirstName;@p3=LastName";

            //Source
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2') IS NOT NULL DROP TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2; CREATE TABLE dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2 (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50)); INSERT dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2 VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1');";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestReiceverDBExecuteStoredProcedureNoParam2') IS NOT NULL DROP PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam2;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestReiceverDBExecuteStoredProcedureNoParam2 AS SELECT * FROM dbo.TestingTestReiceverDBExecuteStoredProcedureNoParam2;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            var rcvr = new ReceiverStmtSqlServer(connectionString, commandType, commandText1, fileNameSerilizeLastRow, parameters1);

            //Destination
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
            var snd = new SenderToDBStmtSqlServer(connectionString, commandType, commandText2, parameters2);

            //Job
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, rcvr);
            job.Senders.Add(0, snd);
            #endregion

            #region act
            job.Execute().Wait(-1);
            #endregion

            #region assert
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) AS Cnt FROM TestingTestSenderDBExecuteStoredProcedureWithParams2 tbl FULL JOIN (VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1')) chk(PersonID, FirstName, LastName) ON tbl.PersonID = chk.PersonID AND tbl.FirstName = chk.FirstName AND tbl.LastName = chk.LastName WHERE EXISTS(SELECT tbl.PersonID EXCEPT SELECT chk.PersonID) OR EXISTS(SELECT tbl.FirstName EXCEPT SELECT chk.FirstName) OR EXISTS(SELECT tbl.LastName EXCEPT SELECT chk.LastName);";
                    var cnt = (int)await cmd.ExecuteScalarAsync();
                    Assert.AreEqual(0, cnt);
                }
            }
            #endregion
        }

        [TestMethod]
        public async Task TestSenderDBExecuteStoredProcedureWithParamsVSTS()
        {
            #region arrange
            connectionString = @"Server=(localdb)\ERROR;Trusted_Connection=True;";
            string commandText = "dbo.TestSenderDBExecuteStoredProcedureWithParams";
            string parameters = "@p1=PersonID;@p2=FirstName;@p3=LastName";

            //Prepare source data: 2 rows {ID, FirstName, LastName}
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

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "IF OBJECT_ID('dbo.TestingTestSenderDBExecuteStoredProcedureWithParams') IS NOT NULL DROP TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams; CREATE TABLE dbo.TestingTestSenderDBExecuteStoredProcedureWithParams (PersonID INT NOT NULL PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50));";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "IF OBJECT_ID('dbo.TestSenderDBExecuteStoredProcedureWithParams') IS NOT NULL DROP PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = "CREATE PROCEDURE dbo.TestSenderDBExecuteStoredProcedureWithParams (@p1 INT, @p2 VARCHAR(50), @p3 VARCHAR(50)) AS INSERT dbo.TestingTestSenderDBExecuteStoredProcedureWithParams (PersonID, FirstName, LastName) VALUES (@p1, @p2, @p3)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            var sender = new SenderToDBStmtSqlServer(connectionString, commandType, commandText, parameters);
            sender.valuesToBeSent = rows.ToArray();
            #endregion

            #region act
            await sender.Send();
            #endregion

            #region assert
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT COUNT(*) AS Cnt FROM TestingTestSenderDBExecuteStoredProcedureWithParams tbl FULL JOIN (VALUES (0, 'John 0', 'Doe 0'), (1, 'John 1', 'Doe 1')) chk(PersonID, FirstName, LastName) ON tbl.PersonID = chk.PersonID AND tbl.FirstName = chk.FirstName AND tbl.LastName = chk.LastName WHERE EXISTS(SELECT tbl.PersonID EXCEPT SELECT chk.PersonID) OR EXISTS(SELECT tbl.FirstName EXCEPT SELECT chk.FirstName) OR EXISTS(SELECT tbl.LastName EXCEPT SELECT chk.LastName);";
                    var cnt = (int)await cmd.ExecuteScalarAsync();
                    Assert.AreEqual(0, cnt);
                }
            }
            #endregion
        }
    }
}
