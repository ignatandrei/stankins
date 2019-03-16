using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using Stankins.Jenkins;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("Jenkins", "")]
    [Trait("ExternalDependency", "Jenkins")]
    public class TestJenkins
    {
        [Scenario]
        [Example("http://localhost:8080", 3)]
        public void TestSimpleJSON(string url, int numberTables)
        {
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(JenkinsJson)} for the {url}".w(() => receiver = new JenkinsJson(url));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });

        }

        [Scenario]
        [Example("http://localhost:8080", 1)]
        public void TestAlive(string url, int numberTables)
        {
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(ReceiverJenkins)} for the {url}".w(() => receiver = new ReceiverJenkins(url));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });
            $"and should have data success".w(() =>
            {
                var res = AliveStatus.FromTable(data.DataToBeSentFurther.First().Value);
                res.Should().NotBeNull();
                res.Count().Should().Be(1);
                res.First().IsSuccess.Should().Be(true);
            });

        }

    }
}
