using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiveDatabasesSql", "")]
    [Trait("ExternalDependency", "SqlServer")]
    public class TestReceiveDatabasesSql
    {
        static TestReceiveDatabasesSql()
        {
            SqlConnection = Environment.GetEnvironmentVariable("sqlserver");
            if (string.IsNullOrWhiteSpace(SqlConnection))
            {
                SqlConnection = "(local)";
            }
            SqlConnection = "Server=" + SqlConnection + "; Database = master; User Id = SA; Password =<YourStrong!Passw0rd>; ";

        }
        public static string SqlConnection;
        
        public static IEnumerable<object[]> SqlServerConnection()
        {
            
            return new List<object[]>
            {
                new object[] {SqlConnection}
            };
        }

        [Scenario]
        [MemberData(nameof(SqlServerConnection))]
        public void SenderToDot(string connectionString)
        {
            IReceive status = null;

            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() =>
            {

            });
            $"When I create the ReceiverDBServer ".w(() => status = new ReceiveDatabasesSql(connectionString));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a databases".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(1);
            });

            $"should be some content".w(() =>
            {
                data.DataToBeSentFurther[0].Rows.Count.Should().BeGreaterOrEqualTo(4);//master,msdb,tempdb,model 

            });



        }
    }
}
