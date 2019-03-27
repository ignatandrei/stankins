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
    [Trait("ReceiverSqlServer", "")]
    [Trait("ExternalDependency","SqlServer")]
    public class TestReceiverSqlServer
    {
        [Scenario]
        [MemberData(nameof(TestReceiveDatabasesSql.SqlServerConnection), MemberType = typeof(TestReceiveDatabasesSql))]
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
            $"and the result should be true".w(() =>
            {
                data.DataToBeSentFurther[0].Rows[0]["IsSuccess"].Should().Be(true);
            });


        }
    }
}
