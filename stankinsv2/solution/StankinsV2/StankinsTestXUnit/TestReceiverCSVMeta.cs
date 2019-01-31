using FluentAssertions;
using Stankins.HTML;
using Stankins.Interfaces;
using System;
using System.IO;
using Xbehave;
using Xunit;
using static System.Environment;
namespace StankinsTestXUnit
{
    [Trait("ReceiverHtmlMeta", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHtmlMeta
    {
        private const string html = @"<head>
  <meta charset='UTF-8'>
  <meta name='description' content='Free Web tutorials'>
  <meta name='keywords' content='HTML,CSS,XML,JavaScript'>
  <meta name='author' content='John Doe'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head><body>

<p>All meta information goes in the head section...</p>

</body>
</html>";
        [Scenario]
        [Example(html, 5)]
        public void TestSimpleHtml(string fileContents,int NumberRows)
        {
            IReceive receiver = null;
            fileContents = fileContents.Replace("{NewLine}", NewLine);
            string fileName =nameof(TestReceiverHtmlMeta) +nameof(TestSimpleHtml);
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName} with Content {fileContents}".w(async () =>
            {
                await File.WriteAllTextAsync(fileName, fileContents);
            });
            $"When I create the receiver html for the {fileName}".w(() => receiver = new ReceiverHtmlMeta(fileName,null));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));


        } 
    }
}
