using CommonDB;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using SenderBulkCopy;
using Shouldly;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestPBX
    {
        private string GetSqlServerConnectionString()
        {
            var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            var configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }
        [TestMethod]
        public async Task TestPBXData()
        {
            #region arrange
            string connectionString = GetSqlServerConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
IF OBJECT_ID('dbo.PBXData', 'U') IS NOT NULL
 DROP TABLE dbo.PBXData;";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = @"CREATE TABLE [PBXData](
	[NewDatePBX] [datetime] NOT NULL,
	[lineNr] [int] NOT NULL,
	[text] [nvarchar](500) NOT NULL,
	[FullName] [nvarchar](500) NOT NULL
)";
                    await cmd.ExecuteNonQueryAsync();
                }
                #endregion
                #region act
                var dir = AppContext.BaseDirectory;
                var dirPBX = Path.Combine(dir, "PBX");
                IReceive r = new ReceiverFolderHierarchical(dirPBX, "*.log");
                IFilter filterFiles = new FilterForFilesHierarchical();
                ITransform transformLines = new TransformerFileToLines() { TrimEmptyLines = true };
                var trDateRegex = new TransformRowRegex(@"^Date:\ (?<datePBX>.{23}).*?$", "text");
                var trToDate = new TransformerFieldStringToDate("datePBX", "NewDatePBX", "yyyy/MM/dd HH:mm:ss.fff");

                var trAddDate = new TransformAddFieldDown("NewDatePBX");
                var trSimpleFields = new TransformRowRemainsProperties("NewDatePBX", "lineNr", "text", "FullName");

                var data = new DBTableDataConnection<SqlConnection>(new SerializeDataInMemory());
                data.ConnectionString = GetSqlServerConnectionString();
                data.Fields = new string[] { "NewDatePBX", "lineNr", "text", "FullName" };
                data.TableName = "PBXData";
                var bulk = new SenderSqlServerBulkCopy(data);

                var si = new SimpleJob();
                si.Receivers.Add(0, r);
                si.FiltersAndTransformers.Add(0, filterFiles);
                si.FiltersAndTransformers.Add(1, transformLines);
                si.FiltersAndTransformers.Add(2, trDateRegex);
                si.FiltersAndTransformers.Add(3, trToDate);
                si.FiltersAndTransformers.Add(4, trAddDate);
                si.FiltersAndTransformers.Add(5, trSimpleFields);
                //TODO: add transformer to add a field down for all fields
                //TODO: add transformer regex for splitting Key=Value
                //TODO: add field to separate Conn(1)Type(Any)User(InternalTask) CDL Request:RSVI(Get)
                si.Senders.Add(0, bulk);

                await si.Execute();
                #endregion
                #region assert
                filterFiles.valuesTransformed.Length.ShouldBe(2, "just two files after first filter");
                transformLines.valuesTransformed.Length.ShouldBe(77251);
                var d = transformLines.valuesTransformed.Select(it => it.Values["FullName"]).Distinct().ToArray();
                d.Length.ShouldBe(2, "two files after reading contents");
                //transformGroupingFiles.valuesTransformed.Length.ShouldBe(2);
                //trDateRegex.valuesTransformed.Count(it => it.Values.ContainsKey("datePBX")).ShouldBeGreaterThan(0,"datePBX");
                //trToDate.valuesTransformed.Count(it => it.Values.ContainsKey("NewDatePBX")).ShouldBeGreaterThan(0,"NewDatePBX");
                foreach (var item in trAddDate.valuesTransformed)
                {
                    item.Values.ShouldContainKey("NewDatePBX");
                }
                foreach (var item in trSimpleFields.valuesTransformed)
                {
                    item.Values.Keys.Count.ShouldBe(4);
                }
                using (var con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select count(*) from PBXData";
                        var val = await cmd.ExecuteScalarAsync();
                        val.ShouldBe(77251);

                    }
                }
                #endregion

            }
        }
    }
}
