using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Stankins.Amazon;
using Stankins.Interfaces;
using Stankins.Version;
using Stankins.XML;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("AmazonMeta", "")]
    [Trait("ExternalDependency", "0")]
    public class TestAmazonMeta
    {
        [Scenario]
        [Example("Assets/HTML/AmazonCopyStackOverflow.html", 5)]
        public void TestAmazonCopyPasteStackOverflow(string fileName,int nrRows)
         {
            IReceive receiver = null;
            IDataToSent data=null;

            $"Given the file {fileName} ".w( () =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the {nameof(AmazonMeta)} for the {fileName}".w(() => receiver = new AmazonMeta(fileName,null));

            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {nrRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRows));

        }
    }
}
