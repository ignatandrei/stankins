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
    [Trait("ReceiverReadFileText", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverReadFileText
    {
        [Scenario]
        [Example("a.txt",1)]
        public void TestSimpleReceive(string newFileName, int numberRows)
        {
            string newFolder = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(Environment.CurrentDirectory, newFolder);
            Directory.CreateDirectory(dir);
            var full = Path.Combine(dir, newFileName);
            File.WriteAllText(full,newFileName);
           
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            $"When I create the receiver files in folder {newFolder}".w(() => receiver = new ReceiverReadFileText(full));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);

            });
            $"The number of rows should be {numberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows));
            $"and the contents should be {newFileName}".w(() =>
                data.DataToBeSentFurther[0].Rows[0]["FileContents"].Should().Be(newFileName));

            $"and the FilePath should be {full}".w(() =>
                data.DataToBeSentFurther[0].Rows[0]["FilePath"].Should().Be(full));

        }
    }
}