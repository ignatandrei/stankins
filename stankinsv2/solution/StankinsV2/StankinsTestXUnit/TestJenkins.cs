using FluentAssertions;
using Stankins.FileOps;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stankins.Rest;
using Xbehave;
using Xunit;
using static System.Environment;
using Stankins.Trello;
using Stankins.Cachet;
using Stankins.Jenkins;

namespace StankinsTestXUnit
{
    [Trait("Jenkins", "")]
    [Trait("ExternalDependency", "Jenkins")]
    public class TestJenkins
    {
        [Scenario]
        [Example("http://localhost:8080", 3)]
        public void TestSimpleJSON(string url,int numberTables)
        {
            IReceive receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
           
            $"When I create the {nameof(JenkinsJson)} for the {url}".w(() => receiver = new JenkinsJson(url));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(numberTables);
            });

        } 

    }
}
