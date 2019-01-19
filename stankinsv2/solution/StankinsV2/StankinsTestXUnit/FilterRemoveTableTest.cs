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
    [Trait("FilterRemoveTableTest", "")]
    [Trait("AfterPublish", "0")]

    public class FilterRemoveTableTest
    {
        [Scenario]
        [Example("Car, Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003{NewLine}Mercedes, 2003")]
        public void TestSimpleCSV(string fileContents)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });

            $"And when I filter".w(async () => data = await new FilterRemoveTable(data.Metadata.Tables[0].Name).TransformData(data));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With 0 table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(0);
            });
            

        }

    }
}
