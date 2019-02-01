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
    [Trait("ReceiverHtmlAHref", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHtmlAHref
    {
        [Scenario]
        [Example("<a href='http://msprogrammer.serviciipeweb.ro/'>MyBlog</a>", 1)]
        public void TestSimple(string fileContents,int numberRows)
        {
            IReceive receiver = null;
            var nl = Environment.NewLine;
            fileContents = fileContents.Replace("{NewLine}", nl);
            string fileName = nameof(TestReceiverHtmlAHref)+nameof(TestSimple);
            IDataToSent data=null;
            
            $"Given the file {fileName} with Content {fileContents}".w(async () =>
            {
                await File.WriteAllTextAsync(fileName, fileContents);
            });
            $"When I create the receiver csv for the {fileName}".w(() => receiver = new ReceiverHtmlAHref(fileName));
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
