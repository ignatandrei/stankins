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
    [Trait("TransformerToOneTable", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformerToOneTableTest
    {
        [Scenario]
        [Example("Car,Year{NewLine}Ford,2000", "Car,Year{NewLine}Mercedes,2001",2,1,2)]
        public void TestSimpleCSV(string fileContents1,string fileContents2,int nrTablesBefore, int nrTablesAfter, int nrRowsAfter)
        {
            IReceive receiver = null;
            IDataToSent data=null;
            var nl = Environment.NewLine;
            fileContents1 = fileContents1.Replace("{NewLine}", nl);
            fileContents2 = fileContents2.Replace("{NewLine}", nl);
            $"When I create the receiver csv for the content {fileContents1}".w(() => receiver = new ReceiverCSVText(fileContents1));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"And I read the data from other receiver".w(async () => data = await new ReceiverCSVText(fileContents2).TransformData(data));

            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {nrTablesBefore} tables".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(nrTablesBefore);
            });
            $"and I transform to one table".w(async () =>  data= await new TransformerToOneTable().TransformData(data));

            $"Should be {nrTablesAfter} tables".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(nrTablesAfter);
            });

            $"with {nrRowsAfter} rows".w(() =>
            {
                data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRowsAfter);
                
            });


        }
    }
}
