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
    [Trait("TestChangeColumnName", "")]
    [Trait("ExternalDependency", "0")]
    public class TestChangeColumnName
    {
        [Scenario]
        [Example("Car,Year{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003", "Car", "NewCar")]
        public void TestSimpleCSV(string fileContents, string fromName, string toName)
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
            $"and the table should contain column name {fromName}".w(()=>{
            data.DataToBeSentFurther[0].Columns.Contains(fromName).Should().BeTrue();
            data.DataToBeSentFurther[0].Columns.Contains(toName).Should().BeFalse();
            
                });

            $"and applying {nameof(ChangeColumnName)} from {fromName} to {toName}".w(async () =>
                data = await new ChangeColumnName(fromName,toName).TransformData(data));

            $"and the table should contain column name {toName}".w(()=>{
            data.DataToBeSentFurther[0].Columns.Contains(toName).Should().BeTrue();
            data.DataToBeSentFurther[0].Columns.Contains(fromName).Should().BeFalse();
            
                });

        }
    }
}