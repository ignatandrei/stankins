using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xbehave;
using Xbehave.Sdk;
using Xunit;

namespace StankinsTestXUnit
{
    static class MyExtensionsXBehave
    {
        public static IStepBuilder w(this string text, Action body)
        {
            Console.WriteLine("!" + text);
            return text.w(body);
        }
    }
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
            $"When I create the receiver csv for the content {fileContents}".w(()=> receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".x(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(()=> data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"And when I filter".x(async () => data = await new FilterColumnDataGreaterThanLength("Car", 5).TransformData(data));
            $"Then should be a data".w(()=> data.Should().NotBeNull());
            $"With a table".w(()=>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRowsAfterFilter}".w(()=> data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRowsAfterFilter));


        }
    }
}
