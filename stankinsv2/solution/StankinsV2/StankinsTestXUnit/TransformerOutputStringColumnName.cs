using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using Stankins.Razor;
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
    [Trait("TransformerOutputStringColumnName", "")]
    [Trait("ExternalDependency", "0")]
    public class TransformerOutputStringColumnNameTest
    {
        [Scenario]
        [Example("Year, Car{NewLine}Ford, 2000{NewLine}Rolls Royce, 2003",2,"'a.html'")]
        public void TestSimpleCSVWithConst(string fileContents,int NumberRows,string newVal)
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

            $"and now I send to razor {nameof(SenderToRazorWithContents)}".w(async () =>
            {
                data = await new SenderToRazorWithContents("asdasdasd").TransformData(data);
            });
            $"and I can transform the value of the output ".w(async () =>
            {
                data = await new TransformerOutputStringColumnName(newVal).TransformData(data);
            });
            $"should be the value equal to {newVal}".w(() =>
            {
                data.FindAfterName("OutputString").Value.Rows[0]["Name"].ToString().Should().Be(newVal.Replace("'",""));
            });

        } 
    }
}
