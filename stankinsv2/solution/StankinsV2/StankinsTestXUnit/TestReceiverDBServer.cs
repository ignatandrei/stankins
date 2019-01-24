using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiverWeb", "")]
    [Trait("ExternalDependency","SqlServer")]
    public class TestReceiverSqlServer
    {
        [Scenario]
        [Example("Server=myServerAddress;Database=master;User Id=SA;Password = <YourStrong!Passw0rd>;")]
        public void TestReceiverDBServer(string connectionString)
        {
            IReceive status = null;
            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ReceiverDBSqlServer(connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a table".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(1);
            });



        }
    }
}
