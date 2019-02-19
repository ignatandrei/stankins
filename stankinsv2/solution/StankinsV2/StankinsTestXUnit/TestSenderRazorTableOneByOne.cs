using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SenderInterpretedRazor;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("SenderRazorTableOneByOne", "")]
    [Trait("ExternalDependency", "0")]
    public class TestSenderRazorTableOneByOne 
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2 )]
        public void TestSimpleCSVRender(string fileContents,int numberRows)
        {
            string newFolder = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(Environment.CurrentDirectory, newFolder);
            Directory.CreateDirectory(dir);
            IReceive receiver = null;
            IDataToSent data=null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);

            });
            $"The number of rows should be {numberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows));
            $"and now use SenderRazorTableOneByOne ".w(async () =>
            {
                data =await  new SenderRazorTableOneByOne("aaa", dir).TransformData(data);

            });
            $"and the new folder {dir} should have 1 file".w(() => { Directory.GetFiles(dir).Length.Should().Be(1); });
        } 
    }
}
