using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("FilterRetainColumnDataContains", "")]
    [Trait("AfterPublish", "0")]
    public class FilterRetainColumnDataContainsTest
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 3,"Car","Rolls",1)]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 3, "Car","Rolx", 0)]
        public void TestSimpleCSV(string fileContents, int numberRows,string filter,string contain, int numberRowsAfterFilter)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(()=> receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With 1 tables and {numberRows} rows".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
                data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows);


            });
            $"And when I filter with {filter} {contain} ".w(async () => data = await new FilterRetainColumnDataContains(filter,contain).TransformData(data));
            $"Then should be a data".w(()=> data.Should().NotBeNull());
            $"With 1 table and {numberRowsAfterFilter} after filter".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
                data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRowsAfterFilter);
            });


        }
    }
}
