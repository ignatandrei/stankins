using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using StanskinsImplementation;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using Transformers;
using StankinsInterfaces;
using Moq;

namespace StankinsTests
{
    [TestClass]
    public class TestTransformDBStmtSqlServer
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
        public async Task TestTransformDBStmtSqlServerSimple()
        {
            //First call
            #region arange
            // arrange source rows
            var rows = new List<IRow>();
            var row0 = new Mock<IRow>();
            row0.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 0,
                    ["TotalInvoice"] = null
                }
            );
            rows.Add(row0.Object);

            var row1 = new Mock<IRow>();
            row1.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 1,
                    ["TotalInvoice"] = 10
                }
            );
            rows.Add(row1.Object);

            var row2 = new Mock<IRow>();
            row2.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 2,
                    ["TotalInvoice"] = 1000
                }
            );
            rows.Add(row2.Object);

            // arrange SQL SP
            string connectionString = GetSqlServerConnectionString();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "DROP PROCEDURE IF EXISTS dbo.ComputeVAT;";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = @"CREATE PROCEDURE dbo.ComputeVAT 
@xmlInputValues XML
AS
BEGIN
SELECT	x.XmlCol.value('(@ID)', 'INT') AS ID, -- IdentifierColumn
		x.XmlCol.value('(@TotalInvoice)', 'INT') * 0.19 AS VATValue -- OutputColumns
FROM	@xmlInputValues.nodes('/root/row') x(XmlCol)
END";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // arrange transformer
            var transform = new TransformDBStmtSqlServer(connectionString, CommandType.StoredProcedure, "dbo.ComputeVAT", TransformDBStmtSqlServer.ExecutionMode.SingleCall, "TotalInvoice", "VATValue", "ID");
            transform.valuesRead = rows.ToArray();
            #endregion

            #region act
            await transform.Run();
            #endregion

            #region assert
            foreach (var item in transform.valuesTransformed)
            {
                Assert.IsTrue(item.Values.ContainsKey("ID"));
                Assert.IsTrue(item.Values.ContainsKey("TotalInvoice"));
                Assert.IsTrue(item.Values.ContainsKey("VATValue"));

                if((int)item.Values["ID"] != 0)
                {
                    Assert.AreEqual((int)item.Values["TotalInvoice"] * (decimal)0.19, (decimal)item.Values["VATValue"]);
                }
            }
            #endregion
        }
    }
}
