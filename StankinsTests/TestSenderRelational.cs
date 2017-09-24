using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDatabaseObjects;
using SenderHTML;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderRelational
    {
        private string GetSqlServerConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            IConfigurationRoot configuration = builder.Build();
            return configuration["SqlServerConnectionString"]; //VSTS SQL Server connection string "(localdb)\MSSQLLocalDB;Trusted_Connection=True;"
        }
        void CreateExportFilesSqlServer(string folderTo)
        {
            if (!Directory.Exists(folderTo))
                Directory.CreateDirectory(folderTo);

      



            string sqlserverFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "sqlserver.cshtml"));
            File.Copy(@"Views\sqlserver.cshtml", sqlserverFile);

            
            string databaseFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "databases.cshtml"));
            File.Copy(@"Views\databases.cshtml", databaseFile);
            databaseFile += ";"; 
            
            string tableFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "tables.cshtml"));
            File.Copy(@"Views\tables.cshtml", tableFile);

            
            string columnFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "columns.cshtml"));
            File.WriteAllText(@"Views\columns.cshtml", columnFile);
            
        }
        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        public async Task TestExportRelationalHTMLSqlServer()
        {
            #region arrange
            string folderName = AppContext.BaseDirectory;
            //CreateExportFilesSqlServer(Path.Combine(folderName,"Views"));
            var connectionString = GetSqlServerConnectionString();
            
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

            var rr = new ReceiverRelationalSqlServer();
            rr.ConnectionString = connectionString;
            string OutputFileName = SimpleJobConditionalTransformersTest.DeleteFileIfExists( Path.Combine(folderName, "relationalSqlServer.html"));
            var sender = new Sender_HTMLRazor("Views/sqlserver.cshtml", OutputFileName);

            var senderViz = new Sender_HTMLRelationViz("Name", OutputFileName);
            var filter = new FilterExcludeRelation("columns", "tables");            
            var job = new SimpleJobConditionalTransformers();
            job.Receivers.Add(0, rr);
            job.AddSender(sender);
            job.Add(filter);
            job.Add(filter, senderViz);
            #endregion
            #region act
            await job.Execute();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(OutputFileName), $"{OutputFileName} must exists");
            Process.Start("explorer.exe", OutputFileName);
            var text = File.ReadAllText(OutputFileName);
            Assert.IsTrue(text.Contains("TestAndrei"), "must contain table testandrei");
            Assert.IsTrue(text.Contains("FirstName"), "must contain column FirstName ");

            #endregion
        }
    }
}
