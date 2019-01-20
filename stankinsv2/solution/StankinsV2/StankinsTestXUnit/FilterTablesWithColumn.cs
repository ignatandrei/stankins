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
    [Trait("FilterTablesWithColumn", "")]
    [Trait("AfterPublish", "0")]
    public class FilterTablesWithColumnTest
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 1,"Car",1)]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 1, "Carx", 0)]
        public void TestSimpleCSV(string fileContents, int numberTables,string filter, int numberTablesAfterFilter)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(()=> receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} tables".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(numberTables);
            });
            $"And when I filter with {filter}".w(async () => data = await new FilterTablesWithColumn(filter).TransformData(data));
            $"Then should be a data".w(()=> data.Should().NotBeNull());
            $"With {numberTablesAfterFilter} table".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(numberTablesAfterFilter);
            });


        }
    }
}
