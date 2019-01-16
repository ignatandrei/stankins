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
        public async Task TestSimpleCSV(string fileContents,int NumberRows, ReceiverCSVFile receiver)
        {
            fileContents = fileContents.Replace("{NewLine}", NewLine);
            string fileName = nameof(TestSimpleCSV);
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName} with Content {fileContents}".x(async () =>
            {
                await File.WriteAllTextAsync(fileName, fileContents);
            });
            $"When I create the receiver csv for the {fileName}".x(() => receiver = new ReceiverCSVFile(fileName));
            $"And I read the data".x(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".x(() => data.Should().NotBeNull());
            $"With a table".x(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".x(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));


        } 
    }
}
