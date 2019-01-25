using FluentAssertions;
using Stankins.File;
using Stankins.HTML;
using Stankins.Interfaces;
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
    [Trait("ReceiverHtmlList", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHtmlList
    {
        [Scenario]
        [Example(@"Assets\otherbooksmarks.html", 4)]
        public void TestSimpleCSV(string filepath,int numberTables)
        {

            IDataToSent data=null;
            
            $"receiving the file {filepath} ".w(async () =>
            {
                data= await new ReceiverHtmlList(filepath, Encoding.UTF8).TransformData(null);
            });
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} tables".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(4);
            });


        } 
    }
}
