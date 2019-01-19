using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("ReceiverCSV", "")]
    [Trait("AfterPublish", "0")]
    public class TestReceiverCSVFile
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2)]
        public void TestSimpleCSV(string fileContents,int NumberRows)
        {
            IReceive receiver = null;
            fileContents = fileContents.Replace("{NewLine}", NewLine);
            string fileName = nameof(TestSimpleCSV);
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName} with Content {fileContents}".w(async () =>
            {
                await File.WriteAllTextAsync(fileName, fileContents);
            });
            $"When I create the receiver csv for the {fileName}".w(() => receiver = new ReceiverCSVFile(fileName));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));


        } 
    }
}
