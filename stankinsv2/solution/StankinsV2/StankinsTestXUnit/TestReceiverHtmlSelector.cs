using FluentAssertions;
using Stankins.File;
using Stankins.HTML;
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
    [Trait("ReceiverHtmlSelector", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverHtmlSelector
    {
        [Scenario]
        [Example(@"Assets/bookmarks_11_17_17N.html", "//dt/a",22)]
        public void TestSimpleCSV(string filepath,string selectgor, int NumberRows)
        {

            IDataToSent data=null;
            
            $"receiving the file {filepath} ".w(async () =>
            {
                data= await new ReceiverHtmlSelector(filepath, Encoding.UTF8, selectgor).TransformData(null);
            });
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));

            $"and verifier is ok".w(async () => data= await new Verifier().TransformData(data));
            

        }
    }
}
