using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("FilterRemoveColumn", "")]
    [Trait("ExternalDependency", "0")]
    public class FilterRemoveColumnTest
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 2,1)]
        public void TestSimpleCSV(string fileContents, int NumberColumns, int NumberColumnsAfterFilter)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(()=> receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of columns should be {NumberColumns}".w(()=> data.DataToBeSentFurther[0].Columns.Count.Should().Be(NumberColumns));
            $"And when I filter".w(async () => data = await new FilterRemoveColumn("Car").TransformData(data));
            $"Then should be a data".w(()=> data.Should().NotBeNull());
            $"With a table".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberColumnsAfterFilter}".w(()=> data.DataToBeSentFurther[0].Columns.Count.Should().Be(NumberColumnsAfterFilter));


        }
    }
}
