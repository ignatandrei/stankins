using System;
using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using StankinsObjects;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("FilterColumnDataWithRegex","")]
    [Trait("ExternalDependency", "0")]
    public class TestFilterColumnDataWithRegex
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2,"[l]",1)]
        public void TestSimpleCSV(string fileContents,int nrRows,string reg, int nrRowsAfter)
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
            $"with {nrRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRows));

            $" and after applying {nameof( FilterColumnDataWithRegex)} ".w(async ()=> data = await new FilterColumnDataWithRegex("Car","[l]").TransformData(data));

            $"the name of the table should be {nrRowsAfter}".w(() =>data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRowsAfter));


        } 
    }
}