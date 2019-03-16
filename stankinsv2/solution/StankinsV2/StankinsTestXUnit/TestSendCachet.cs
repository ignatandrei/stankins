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

namespace StankinsTestXUnit
{
    [Trait("Cachet", "")]
    [Trait("ExternalDependency", "Cachet")]
    public class TestSenderCachet
    {
        [Scenario]
        [Example("Assets/JSON/CachetV1Simple.txt", 3)]
        public void TestSimpleJSON(string fileName,int NumberRows)
        {
            IReceive receiver = null;
           
            IDataToSent data=null;
            var nl = Environment.NewLine;
            $"Given the file {fileName}".w(() =>
            {
                File.Exists(fileName).Should().BeTrue();
            });
            $"When I create the {nameof(ReceiveRest)} for the {fileName}".w(() => receiver = new ReceiveRestFromFile(fileName));
            $"And I read the data".w(async () =>data= await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With a table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(1);
            });
            $"The number of rows should be {NumberRows}".w(() => data.DataToBeSentFurther[0].Rows.Count.Should().Be(NumberRows));
            $"and now I transform with {nameof(SenderCachet)}".w(async ()=>
                data=await new SenderCachet("http://localhost:8000","5DiHQgKbsJqck4TWhMVO").TransformData(data)
            );

        } 

    }
}
