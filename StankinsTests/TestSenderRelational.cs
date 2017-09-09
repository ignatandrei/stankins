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
            string sqlserver = @"@using System.Linq;
            @using StankinsInterfaces;
            @model StankinsInterfaces.IRow[]

            <h1> Servers Number Rows: @Model.Length</h1>
<table border='1' id='server'>
<tr><th>Nr</th><th>Name</th></tr>
@{
int idRow=1;
}
@foreach(var server in Model){
    
    <tr id='server_@server.Values[" + "\"PathID\"" + @"]'>
<td>@(idRow++)</td>
    <td>
        @server.Values[" + "\"Name\""+ @"]
        <table>
            <tr>
            <td>
            @{ 
            var item= server as IRowReceiveRelation;            
            var child= new Tuple<object,StankinsInterfaces.IRow[]>(item,item.Relations[" + "\"databases\"].ToArray());"+
            @"Html.RenderPartial(" + "\"databases.cshtml\"" + @",child); 

            }
            
            </td>
            <tr>
        </table>
    </td>
    </tr>
}
</table>
";
            string sqlserverFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "sqlserver.cshtml"));
            File.WriteAllText(sqlserverFile, sqlserver);

            string databases= @"@using System.Linq;
            @using StankinsInterfaces;
            @model Tuple<object,StankinsInterfaces.IRow[]>
@{
IRow parent =Model.Item1 as IRow;
int idRow=1;
}
            <h1> databases for server @parent.Values[" + "\"Name\"" + @"] ;

<table border='1' id='databases'>
<tr><th>Nr</th><th>Name</th></tr>

@foreach(var database in Model.Item2){

    <tr id='database_@database.Values[" + "\"PathID\"" + @"]'>
<td>@(idRow++)</td>
    <td>
<a href='#tables_@database.Values[" + "\"PathID\"" + @"]'>
        @database.Values[" + "\"Name\"" + @"]</a>

        <table>
            <tr>
            <td>
            @{ 
            var item= database as IRowReceiveRelation;            
            var child= new Tuple<object,StankinsInterfaces.IRow[]>(item,item.Relations[" + "\"tables\"].ToArray());" +
            @"Html.RenderPartial(" + "\"tables.cshtml\"" + @",child); 
            }
            </td>
            <tr>
        </table>
    </td>
    </tr>
}
</table>

";
            string databaseFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "databases.cshtml"));
            File.WriteAllText(databaseFile, databases);

            string tables = @"@using System.Linq;
            @using StankinsInterfaces;
            @model Tuple<object,StankinsInterfaces.IRow[]>
@{
IRow parent =Model.Item1 as IRow;
int idRow=1;
}
            <h1> tables for database <a href='#database_@parent.Values[" + "\"PathID\"" + @"]'> @parent.Values[" + "\"Name\"" + @"]</a>

<table border='1' id='tables_@parent.Values[" + "\"PathID\"" + @"]'> 

<tr>
<th>Nr</th>
<th>Name</th></tr>

@foreach(var table in Model.Item2){

    <tr id='table_@table.Values[" + "\"PathID\"" + @"]'>
<td>@(idRow++)</td>
    <td>
        @table.Values[" + "\"Name\"" + @"]
 <table>
            <tr>
            <td>
            @{ 
            var item= table as IRowReceiveRelation;            
            var child= new Tuple<object,StankinsInterfaces.IRow[]>(item,item.Relations[" + "\"columns\"].ToArray());" +
            @"Html.RenderPartial(" + "\"columns.cshtml\"" + @",child); 
            }
            </td>
            <tr>
        </table>
    </td>
    </tr>
}
</table>

";
            string tableFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "tables.cshtml"));
            File.WriteAllText(tableFile, tables);

            string columns = @"@using System.Linq;
            @using StankinsInterfaces;
            @model Tuple<object,StankinsInterfaces.IRow[]>
@{
IRow parent =Model.Item1 as IRow;
int idRow=1;
}

            <h1> columns for table <a href='#table_@parent.Values[" + "\"PathID\"" + @"]'> @parent.Values[" + "\"Name\"" + @"]</a>

<table border='1' id='columns'>
<tr><th>Nr</th><th>Name</th></tr>

@foreach(var item in Model.Item2){

    <tr id='column_@item.Values[" + "\"PathID\"" + @"]'>
<td>@(idRow++)</td>
    <td>
        @item.Values[" + "\"Name\"" + @"]
    </td>
    </tr>
}
</table>

";
            string columnFile = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(folderTo, "columns.cshtml"));
            File.WriteAllText(columnFile, columns);
            
        }
        [TestMethod]
        [TestCategory("RequiresSQLServer")]
        public async Task TestExportRelationalHTMLSqlServer()
        {
            #region arrange
            string folderName = AppContext.BaseDirectory;
            CreateExportFilesSqlServer(Path.Combine(folderName,"Views"));
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
            string OutputFileName = SimpleJobConditionalTransformersTest.DeleteFileIfExists( Path.Combine(folderName, "a.html"));
            var sender = new Sender_HTMLRelationViz("Views/sqlserver.cshtml","sqlserver", OutputFileName);
            var job = new SimpleJob();
            job.Receivers.Add(0, rr);
            job.Senders.Add(0, sender);
            #endregion
            #region act
            await job.Execute();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(OutputFileName), $"{OutputFileName} must exists");
            //Process.Start("explorer.exe", OutputFileName);
            var text = File.ReadAllText(OutputFileName);
            Assert.IsTrue(text.Contains("TestAndrei"), "must contain table testandrei");
            Assert.IsTrue(text.Contains("FirstName"), "must contain column FirstName ");

            #endregion
        }
    }
}
