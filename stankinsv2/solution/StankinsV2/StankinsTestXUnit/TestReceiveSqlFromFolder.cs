using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Stankins.Interfaces;
using Stankins.SqlServer;
using StankinsTestXUnit;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ExternalDependency", "SqlServer")]
    public  class TestReceiveSqlFromFolder
    {
        public static IEnumerable<object[]> SelectFromDbData()
        {

            return new List<object[]>
            {
                new object[] { TestReceiveDatabasesSql.SqlConnection, 3, 1, "select 234 as val",
            "234", "select 1000 as val", "1000" }
            };
        }
        [Scenario]
        [Trait("ReceiveQueryFromFolderSql", "")]
        [MemberData(nameof(SelectFromDbData))]
        public void SelectFromDb(string connectionString,int nrTables, int nrRows, string select1, string val1, string select2,
            string val2)
        {
            var pathName = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pathName);
            File.WriteAllText(Path.Combine(pathName, "a1.sql"), select1);
            File.WriteAllText(Path.Combine(pathName, "a2.sql"), select2);
            IReceive status = null;

            IDataToSent data = null;
            $"Assume Sql Server instance {connectionString} exists , if not see docker folder".w(() => { });
            $"When I create the ReceiveQueryFromFileSql ".w(() =>
                status = new ReceiveQueryFromFolderSql(pathName, "*.sql", connectionString));
            $"and receive data".w(async () => { data = await status.TransformData(null); });
            $"the data should have {nrTables} table".w(() =>
            {
                data.DataToBeSentFurther.Count.Should().Be(nrTables); // first table files, other table sql's
            });


            $"should be {nrRows} rows".w(() =>
            {
                data.FindAfterName(select1).Value.Rows.Count.Should().Be(nrRows);
                data.FindAfterName(select2).Value.Rows.Count.Should().Be(nrRows);
                
                
            });

            $"and the result should contain {val1} ".w(() =>
            {
                data.FindAfterName(select1).Value.Rows[0][0]?.ToString().Should().Be(val1);
                data.FindAfterName(select2).Value.Rows[0][0]?.ToString().Should().Be(val2);
            });
        }
    }
}
