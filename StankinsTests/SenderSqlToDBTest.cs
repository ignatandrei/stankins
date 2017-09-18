using CommonDB;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using SenderToDBSqlServer;
using Shouldly;
using StanskinsImplementation;
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

    public class SenderSqlToDBTest
    {
        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }
        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        public async Task TestSqlFolder()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            var conection = GetSqlServerConnectionString();
            var folderSql = Path.Combine(dir, "SqlToExecute");
            var receiverFolder = new ReceiverFolderHierarchical(folderSql, "*.txt");
            var con = new DBDataConnection<SqlConnection>();
            con.ConnectionString = conection;
            var senderSqlServer = new SenderSqlToDBSqlServer(con);
            #endregion
            #region act
            var j = new SimpleJob();
            j.Receivers.Add(0, receiverFolder);
            j.Senders.Add(0, senderSqlServer);
            await j.Execute();
            #endregion
            #region assert
            using(var conn = new SqlConnection(conection))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select count(ID) from TestAndrei";
                    var res = await cmd.ExecuteScalarAsync();
                    res.ShouldNotBeNull();
                    var text = res.ToString();
                    text.ShouldBe("1");
                }
            }
            #endregion
        }
    }
}
