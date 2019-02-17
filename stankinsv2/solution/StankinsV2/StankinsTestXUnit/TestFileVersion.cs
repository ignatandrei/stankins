using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Stankins.Interfaces;
using Stankins.Version;
using Stankins.XML;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("FileVersionFromDir ", "")]
    [Trait("ExternalDependency", "0")]
    public class TestFileVersion
    {
        [Scenario]
        [Example("Assets/dll", 1)]
        public void TestFileVersionInAssets(string folder, int nrRows)
        {
            IReceive receiver = null;
            IDataToSent data=null;

            $"Given the file {folder} ".w( () =>
            {
                Directory.Exists(folder).Should().BeTrue();
            });
            $"When I create the {nameof(FileVersionFromDir)} for the {folder}".w(() => receiver = new FileVersionFromDir(folder));

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
