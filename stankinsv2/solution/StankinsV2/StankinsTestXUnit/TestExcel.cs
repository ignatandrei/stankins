using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.Office;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("SenderExcel", "")]
    [Trait("ExternalDependency", "0")]
    public class TestSenderExcel
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2,"a.xlsx")]
        public void TestSimpleCSVToExcel(string fileContents,int NumberRows,string fileName)
        {
            IReceive receiver = null;
            IDataToSent data=null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            fileName = Guid.NewGuid().ToString("N") + fileName;
            $"the Excel {fileName} should not exists".w(() => File.Exists(fileName).Should().BeFalse());
            $"When I create the receiver csv for the content {fileContents}".w(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"and now I want to export to excel file ".w(async () =>
                data = await new SenderExcel(fileName).TransformData(data));
            $"the Excel {fileName} should exists".w(() => File.Exists(fileName).Should().BeTrue());
        } 
    }
}
