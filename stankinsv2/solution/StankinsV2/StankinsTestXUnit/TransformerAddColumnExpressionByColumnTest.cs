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
    [Trait("TransformTrim", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformerAddColumnExpressionByColumnTest
    {
        [Scenario]
        [Example("Car,Year{NewLine}Ford,2000","Car","'a'+Car","Ford","aFord")]
        public void TestSimpleCSV(string fileContents,string columnExisting, string newCol, string existingValue, string newValue)
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
            $"and I transform from {columnExisting} to {newCol}".w(async () =>  data= await new TransformerAddColumnExpressionByColumn(columnExisting,newCol,columnExisting+"_New" ).TransformData(data));
          
            $"The first row should have the values {existingValue} and {newValue}".w(() => {
                data.DataToBeSentFurther[0].Rows[0][columnExisting].Should().Be(existingValue);
                data.DataToBeSentFurther[0].Rows[0][columnExisting + "_New"].Should().Be(newValue);

            });



        }
    }
}
