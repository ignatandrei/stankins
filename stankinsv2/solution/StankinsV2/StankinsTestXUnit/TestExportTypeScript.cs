using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.Rest;
using Xbehave;
using Xunit;
using static System.Environment;
using Stankins.Trello;
using Stankins.Razor;

namespace StankinsTestXUnit
{
    [Trait("ReceiveRest", "")]
    [Trait("ExternalDependency", "0")]
    public class TestExportTypeScript
    {
        [Scenario]
        [Example("Assets/JSON/json1.txt",2)]
        [Example("Assets/JSON/json1Record.txt",1)]
        public void TestSimpleJSON(string fileName,int NumberRows)
        {
            IReceive receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName}".w(() =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the ReceiveRest for the {fileName}".w(() => receiver = new ReceiveRestFromFile(fileName));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"and now I export to typescript definition ".w(async ()=> 
                        data= await new SenderToTypeScriptDefinition().TransformData(data) );

            $"should have 1 record in ExportOutput".w(()=>
                data.FindAfterName("OutputString").Value.Rows.Count.Should().Be(1));
        } 
         

    }
}
