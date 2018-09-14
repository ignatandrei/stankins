using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDatabaseObjects;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverRelationalSqlServer
    {
        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }
        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        public async Task TestReceiveTablesAndColumns()
        {
            #region arrange
            var connectionString = GetSqlServerConnectionString();
            var rr = new ReceiverRelationalSqlServer();
            rr.ConnectionString = connectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
IF OBJECT_ID('dbo.TestAndrei', 'U') IS NOT NULL
 DROP TABLE dbo.TestAndrei;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = @"create table TestAndrei(
        ID int,
        FirstName varchar(64) not null
             )";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            #endregion
            #region act
            await rr.LoadData();
            #endregion
            #region assert
            Assert.AreEqual(1, rr.valuesRead?.Length);
            var server = rr.valuesRead[0] as IRowReceiveRelation;
            Assert.IsNotNull(server);
            Assert.IsTrue(server.Relations.ContainsKey("databases"));
            var dbs = server.Relations["databases"];
            Assert.IsTrue(dbs.Count > 0, "must contain at least 1 databases");
            int nrTables = 0;
            int nrColumns = 0;
            foreach(var item in dbs)
            {
                var dbsTables = item.Relations["tables"];
                nrTables += dbsTables.Count;
                foreach (var table in dbsTables)
                {
                    var cols = table.Relations["columns"];
                    nrColumns += cols.Count;
                }
            }
            Assert.IsTrue(nrTables > 0, "must contain some tables");
            Assert.IsTrue(nrColumns > 0, "must contain some columns");
            #endregion
        }
    }
}
