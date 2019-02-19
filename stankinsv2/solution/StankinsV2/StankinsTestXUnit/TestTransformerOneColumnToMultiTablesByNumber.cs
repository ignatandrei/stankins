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
    [Trait("TransformerOneColumnToMultiTablesByNumber", "")]
    [Trait("ExternalDependency", "0")]
    public class TestTransformerOneColumnToMultiTablesByNumber
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2,1,3)]
        public void TestSimpleCSV(string fileContents,int NumberRows, int splitRows, int nrTablesAfter)
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
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"and now I transform with {nameof(TransformerOneColumnToMultiTablesByNumber)}".w(async () => data = await new TransformerOneColumnToMultiTablesByNumber(data.Metadata.Tables[0].Name, splitRows).TransformData(data));
            $"and now should be {nrTablesAfter} tables".w(() => data.DataToBeSentFurther.Count.Should().Be(nrTablesAfter));



        } 
    }
}
