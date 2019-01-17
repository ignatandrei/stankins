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
    [Trait("FilterColumnDataGreaterThanLength", "")]
    [Trait("AfterPublish", "0")]
    public class FilterColumnDataGreaterThanLengthTest
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003", 3,1)]
        public void TestSimpleCSV(string fileContents, int NumberRows, int NumberRowsAfterFilter)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".x(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".x(async () => data = await receiver.TransformData(null));
            $"Then should be a data".x(() => data.Should().NotBeNull());
            $"With a table".x(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".x(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"And when I filter".x(async () => data = await new FilterColumnDataGreaterThanLength("Car", 5).TransformData(data));
            $"Then should be a data".x(() => data.Should().NotBeNull());
            $"With a table".x(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRowsAfterFilter}".x(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRowsAfterFilter));


        }
    }
}
