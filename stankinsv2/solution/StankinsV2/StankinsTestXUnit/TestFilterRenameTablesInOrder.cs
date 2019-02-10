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
    [Trait("FilterRenameTablesInOrder", "")]
    [Trait("ChangeTableNamesRegex", "")]
    [Trait("FilterColumnDataWithRegex", "")]
    [Trait("ExternalDependency", "0")]
    public class TestFilterRenameTablesInOrder
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003", 2, "test 0#", "0#")]
        public void TestSimpleCSV(string fileContents, int nrStart, string format, string formatAfterRegex)
        {
            IReceive receiver = null;
            IDataToSent data = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(() =>
                receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"and applying {nameof(FilterRenameTablesInOrder)} with {nrStart} and {format}".w(async () =>
                data = await new FilterRenameTablesInOrder(nrStart, format).TransformData(data));

            $"the name of the table should be {nrStart.ToString(format)}".w(() =>
                data.DataToBeSentFurther[0].TableName.Should().Be(nrStart.ToString(format)));

            $" and after applying {nameof(ChangeTableNamesRegex)} ".w(async () =>
                data = await new ChangeTableNamesRegex("(?:.+ )((?<name>.+))").TransformData(data));

            $"the name of the table should be {nrStart}".w(() =>
                data.DataToBeSentFurther[0].TableName.Should().Be(nrStart.ToString(formatAfterRegex)));
        }
    }
}