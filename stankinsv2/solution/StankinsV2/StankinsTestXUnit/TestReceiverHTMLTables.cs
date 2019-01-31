using FluentAssertions;
using Stankins.File;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.HTML;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("ReceiverCSV", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHTMLTables
    {
        [Scenario]
        [Example("<table><tr><td>id</td></tr><tr><td>1</td></tr></table>",1)]
        public void TestSimpleTable(string fileContents,int numberRows)
        {
            IReceive receiver = null;
            
            string fileName =nameof(TestReceiverHTMLTables) +nameof(TestSimpleTable);
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName} with Content {fileContents}".w(async () =>
            {
                await File.WriteAllTextAsync(fileName, fileContents);
            });
            $"When I create the receiver html for the {fileName}".w(() => receiver = new ReceiverHtmlTables(fileName,null));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {numberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows));


        } 
    }
}
