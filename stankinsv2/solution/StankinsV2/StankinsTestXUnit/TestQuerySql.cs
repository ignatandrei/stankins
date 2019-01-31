using FluentAssertions;
using Stankins.Interfaces;
using Stankins.SqlServer;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ExternalDependency", "SqlServer")]
    public class TestQuerySql
    {
        [Scenario]
        [Trait("ReceiveQueryFromDatabaseSql", "")]
        [Example("Server=(local);Database=msdb;User Id=SA;Password = <YourStrong!Passw0rd>;", "select 234", 1)]
        public void SelectFromDb(string connectionString, string select, int nrRows)
        {
            IReceive status = null;

            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => {

            });
            $"When I create the ReceiveQueryFromDatabaseSql ".w(() => status = new ReceiveQueryFromDatabaseSql(connectionString, select));
            $"and receive data".w(async () =>
            {
                data = await status.TransformData(null);
            });
            $"the data should have a table".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(1);
            });


            $"should be {nrRows} rows".w(() => { data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRows); });



        }
    }
}