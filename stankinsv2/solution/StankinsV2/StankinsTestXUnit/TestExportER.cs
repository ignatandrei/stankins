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
    [Trait("SenderToDot", "")]
    [Trait("ExternalDependency","SqlServer")]
    public class TestExportER
    {
        [Scenario]
        [Example("Server=(local);Database=master;User Id=SA;Password = <YourStrong!Passw0rd>;")]
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
            $"the data should have a tables, columns, relations".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(3);
            });
           
            $"and now export to SenderToDot".w(async () => {
                sender = new SenderDBDiagramToDot("");
                data = await sender.TransformData(data);
            });
            $"should be some content".w(() =>
            {
                sender.OutputContents.Should().NotBeNull();
                sender.OutputContents.Length.Should().Be(1);
                //File.WriteAllText("a.txt",sender.OutputContents.First().Value);
                //Process.Start("notepad.exe","a.txt");

            });



        }
    }
}
