using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using StankinsObjects;
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
    [Trait("AddColumnRegex", "")]
    [Trait("ExternalDependency", "0")]
    public class TestAddColumnRegex
    {
        [Scenario]
        [Example(@"Car_Year{NewLine}Ford/2000{NewLine}Rolls Royce/2003",1,2)]
        public void TestSimple(string fileContents,int NumberCols, int NumberColsAfter, string fRow1,string fRow2)
        {
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
            $"The number of cols should be {NumberCols}".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(NumberCols));
            $"And I applyu the data".w(async () =>data= await new AddColumnRegex("Car", @"(?:.+\/)((?<nameAuthor>.+))").TransformData(null));
            $"The number of cols should be {NumberCols}".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(NumberCols));
            $"The first row should have {fRow1} and {fRow2}".w(() =>{ 
                data.DataToBeSentFurther[0].Rows[0][0].Should().Be(fRow1);
                data.DataToBeSentFurther[0].Rows[0][1].Should().Be(fRow2);});


        } 
    }
}
