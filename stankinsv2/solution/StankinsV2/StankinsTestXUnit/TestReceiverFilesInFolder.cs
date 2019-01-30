using System;
using System.IO;
using FluentAssertions;
using SenderInterpretedRazor;
using Stankins.File;
using Stankins.Interfaces;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiverFilesInFolder", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverFilesInFolder
    {
        [Scenario]
        [Example("a.txt",1)]
        public void TestSimpleReceive(string newFileName, int numberRows)
        {
            string newFolder = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(Environment.CurrentDirectory, newFolder);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir,newFileName),newFileName);
           
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            $"When I create the receiver files in folder {newFolder}".w(() => receiver = new ReceiverFilesInFolder(dir,"*.txt"));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);

            });
            $"The number of rows should be {numberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows));
            
        }
    }
}