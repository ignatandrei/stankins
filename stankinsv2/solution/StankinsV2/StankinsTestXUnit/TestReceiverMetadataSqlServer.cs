using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using Stankins.SqlServer;
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
    public class TestReceiveMetadataFromDatabaseSql
    {
        [Scenario]
        [MemberData(nameof(TestReceiveDatabasesSql.SqlServerConnection), MemberType = typeof(TestReceiveDatabasesSql))]
        public void TestReceiverDBServer(string connectionString)
        {
            IReceive status = null;
            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ReceiveMetadataFromDatabaseSql(connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have tables, columns, relations,keys,properties".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(5);
            });
          

        }
    }
}
