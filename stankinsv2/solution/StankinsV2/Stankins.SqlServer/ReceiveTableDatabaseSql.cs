using StankinsCommon;

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

                {nameof(sql),$"select * from {nameTable}"}

            })

        {

            Name = nameof(ReceiveTableDatabaseSql);
        }
    }
}