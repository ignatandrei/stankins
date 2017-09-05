using CommonDB;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SenderBulkCopy;
using StankinsInterfaces;
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
    public class TestBulkCopy
    {
        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }
        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        public async Task TestSqlBulkCopyMockData()
        {
            #region arrange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei" + i,
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }
            string connectionString = GetSqlServerConnectionString();
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
            var data = new DBTableDataConnection<SqlConnection>(new SerializeDataInMemory());
            data.ConnectionString = GetSqlServerConnectionString();
            data.Fields = new string[] { "ID", "FirstName" };
            data.TableName = "TestAndrei";
            var bulk = new SenderSqlServerBulkCopy(data);
            bulk.valuesToBeSent = rows.ToArray();
            await bulk.Send();

            #endregion
            #region assert
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select count(*) from TestAndrei";
                    var val = await cmd.ExecuteScalarAsync();
                    Assert.AreEqual(val, nrRows);
                    cmd.CommandText = "select sum(ID) from TestAndrei";
                    val = await cmd.ExecuteScalarAsync();
                    //sum from 0 to nrRows-1
                    Assert.AreEqual((nrRows) * (nrRows - 1) / 2, val);
                }
            }
            #endregion
        }
    }
}
