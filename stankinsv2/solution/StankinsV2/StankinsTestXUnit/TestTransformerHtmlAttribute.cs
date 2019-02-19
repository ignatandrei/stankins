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
    [Trait("TransformerHtmlAttribute", "")]
    [Trait("ExternalDependency", "0")]
    public class TestTransformerHtmlAttribute
    {
        [Scenario]
        [Example(@"Assets/bookmarks_11_17_17.html",2,3)]
        public void TestSimple(string fileName,int numberColsBefore,int numberColsAfter)
        {

            
            //data = await new TransformerHTMLAttribute("item_html", "href").TransformData(data);
            IReceive receiver = null;
            var nl = Environment.NewLine;
           
            
            IDataToSent data=null;
            
            $"Given the file {fileName} ".w( () => { File.Exists(fileName).Should().BeTrue(); });
            $"When I create the ReceiverHtmlSelector  for the {fileName}".w(() => 
                receiver = new ReceiverHtmlSelector(fileName, Encoding.UTF8, "//dt/a"));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of cols should be {numberColsBefore}".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(numberColsBefore));
            $"and I transform data with TransformerHtmlAttribute".w(async () =>
                data = await new TransformerHTMLAttribute("item_html", "href").TransformData(data));
            $"The number of cols should be {numberColsBefore}".w(() => data.DataToBeSentFurther[0].Columns.Count.Should().Be(numberColsAfter));


        } 
    }
}
