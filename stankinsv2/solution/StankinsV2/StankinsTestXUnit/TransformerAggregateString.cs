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
    [Trait("TransformerAggregateString", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformerAggregateStringTest
    {
        [Scenario]
        [Example("Car,Year{NewLine}Ford,2000{NewLine}Mercedes,2001","Year", "2000,2001")]
        public void TestSimpleCSV(string fileContents,string columnExisting, string newValue)
        {
            IReceive receiver = null;
            IDataToSent data=null;
            var nl = "\n";
            fileContents = fileContents.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents}".w(() => receiver = new ReceiverCSVText(fileContents));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"and I aggregate all values from {columnExisting} ".w(async () =>  data= await new TransformerAggregateString(columnExisting,",").TransformData(data));
          
            $"The first row should have the values  {newValue}".w(() => {
                data.DataToBeSentFurther[1].Rows[0][0].Should().Be(newValue);

            });



        }
    }
}
