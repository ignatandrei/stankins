using FluentAssertions;
using Stankins.FileOps;
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
    [Trait("TransformTrim", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformTrimTest
    {
        [Scenario]
        [Example("Car,Year{NewLine} Ford ,2000"," Ford ")]
        public void TestSimpleCSV(string fileContents,string nameWithSpaces)
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
            $"The first row should have spaces {nameWithSpaces}".w(() => data.DataToBeSentFurther[0].Rows[0]["Car"].Should().Be(nameWithSpaces));
            $"and applying {nameof(TransformTrim)}".w(async () => data = await new TransformTrim().TransformData(data));
            $"The first row should have no spaces {nameWithSpaces.Trim()}".w(() => data.DataToBeSentFurther[0].Rows[0]["Car"].Should().Be(nameWithSpaces.Trim()));



        }
    }
}
