using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("TransformSplitColumn ", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformSplitColumnTest
    {
        [Scenario]
        [Example("TwoCols{NewLine}Ford 2000{NewLine}Mercedes 2010",1,3)]
        public void TestSimpleCSV(string fileContents,int numberColsBefore, int numberColsAfter)
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
           
            $"that have {numberColsBefore} columns".w(() => data.DataToBeSentFurther.First().Value.Columns.Count.Should().Be(numberColsBefore));
            $"and applying {nameof(TransformSplitColumn)}".w(async () => data = await new TransformSplitColumn(data.Metadata.Tables[0].Name, "TwoCols",' ').TransformData(data));
            $"and now have {numberColsAfter} columns".w(() => data.DataToBeSentFurther.First().Value.Columns.Count.Should().Be(numberColsAfter));



        }
    }
}
