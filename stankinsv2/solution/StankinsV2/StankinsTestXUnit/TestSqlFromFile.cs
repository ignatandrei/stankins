using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
   
    [Trait("ExternalDependency","SqlServer")]
    public class TestReceiveQueryFromFileSql
    {
       
        [Scenario]
        [Trait("ReceiveQueryFromFileSql", "")]
        [Example("Server=(local);Database=msdb;User Id=SA;Password = <YourStrong!Passw0rd>;","select 234 as val", 1, "234")]
        public void SelectFromDb(string connectionString, string select, int nrRows, string result)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString("N"));
            File.WriteAllText(fileName,select);
            IReceive status = null;
           
            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiveQueryFromFileSql ".w(() => status = new ReceiveQueryFromFileSql( fileName, connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a table".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(2); // first table file, second table sql
            });

          
            $"should be {nrRows} rows".w(() => { data.DataToBeSentFurther[1].Rows.Count.Should().Be(nrRows); });

            $"and the result should be {result} ".w(() => { data.DataToBeSentFurther[1].Rows[0][0]?.ToString().Should().Be(result); });

        }
        [Scenario]
        [Trait("SenderToDot", "")]
        [Example("Server=(local);Database=msdb;User Id=SA;Password = <YourStrong!Passw0rd>;")]
        public void SenderToDot(string connectionString)
        {
            IReceive status = null;
            ISenderToOutput sender = null;
            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ReceiveMetadataFromDatabaseSql(connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a tables, columns, relations,keys, properties".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(5);
            });
           
            $"and now export to SenderToDot".w(async () => {
                sender = new SenderDBDiagramToDot("");
                data = await sender.TransformData(data);
            });
            $"should be some content".w(() =>
            {
                sender.OutputString.Should().NotBeNull();
                sender.OutputString.Rows.Count.Should().Be(1);
                //File.WriteAllText(@"C:\Users\Surface1\Desktop\viz.html", sender.OutputContents.First().Value);
                //Process.Start("notepad.exe","a.txt");

            });



        }
        [Scenario]
        [Trait("SenderDBDiagramHTMLDocument", "")]
        [Example("Server=(local);Database=msdb;User Id=SA;Password = <YourStrong!Passw0rd>;")]
        public void SenderToHTML(string connectionString)
        {
            IReceive status = null;
            ISenderToOutput sender = null;
            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ReceiveMetadataFromDatabaseSql(connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a tables, columns, relations,keys, properties".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(5);
            });

            $"and now export to SenderToDot".w(async () => {
                sender = new SenderDBDiagramHTMLDocument("");
                data = await sender.TransformData(data);
            });
            $"should be some content".w(() =>
            {
                sender.OutputString.Should().NotBeNull();
                sender.OutputString.Rows.Count.Should().Be(1);
                //File.WriteAllText(@"C:\Users\Surface1\Desktop\viz.html", sender.OutputContents.First().Value);
                //Process.Start("notepad.exe","a.txt");

            });



        }
    }
}
