using StankinsCommon;
using System.Data.SqlClient;

namespace Stankins.SqlServer
{
    public class ReceiveTableDatabaseSql : ReceiveQueryFromDatabaseSql
    {
        public ReceiveTableDatabaseSql(CtorDictionary dict) : base(dict)
        {

            Name = nameof(ReceiveTableDatabaseSql);

        }
        public ReceiveTableDatabaseSql(string connectionString, string nameTable) :
            this(
            new CtorDictionary()
            {
                {nameof(connectionString), connectionString},
                {nameof(connectionType),typeof(SqlConnection).FullName},
                {nameof(sql),$"select * from {nameTable}"}

            })

        {

            Name = nameof(ReceiveTableDatabaseSql);
        }
    }
}