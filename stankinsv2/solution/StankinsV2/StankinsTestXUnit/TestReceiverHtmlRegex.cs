using FluentAssertions;
using Stankins.FileOps;
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
    [Trait("ReceiverHtmlRegex", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHtmlRegex
    {
        [Scenario]
        [Example(@"Assets/bg.html", @"\b(?<FirstWord>\w+)\s?((\w+)\s)*(?<LastWord>\w+)?(?<Punctuation>\p{Po})", 3, 1)]//https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.getgroupnames?view=netframework-4.7.2
        public void TestSimple(string fileName,string regEx,int numberCols,int numberRows)
        {

            
          
            IReceive receiver = null;
            var nl = Environment.NewLine;
           
            
            IDataToSent data=null;
            
            $"Given the file {fileName} ".w( () => { File.Exists(fileName).Should().BeTrue(); });
            $"When I create the ReceiverHtmlRegex  for the {fileName}".w(() => 
                receiver = new ReceiverHtmlRegex(fileName, Encoding.UTF8,regEx));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of cols should be {numberCols}".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(numberCols));
            $"The number of rows should be {numberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(numberRows));



        }
    }
}
