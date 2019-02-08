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
    public class TestExportER
    {
        [Scenario]
        [Trait("ExportDBDiagramHtmlAndDot", "")]
        [Example("Server=(local);Database=msdb;User Id=SA;Password = <YourStrong!Passw0rd>;", "a.html")]
        public void TestExportDBDiagramHtmlAndDot(string connectionString, string fileName)
        {
            IBaseObject status = null;
            IDataToSent data = null;
            fileName = Guid.NewGuid().ToString("N") + fileName;
            $"Assume file does not exists".w(() => File.Exists(fileName).Should().BeFalse());
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ExportDBDiagramHtmlAndDot(connectionString,fileName));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });

            $"the data should have a tables, columns, relations,keys, properties".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(7);
            });


            $"Assume file  exists".w(() => File.Exists(fileName).Should().BeTrue());



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
            $"the data should have a tables, columns, relations,keys,properties".w(() =>
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
            $"the data should have a tables, columns, relations,keys,properties".w(() =>
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
