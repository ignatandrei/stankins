using Stankins.Interfaces;
using StankinsCommon;

namespace Stankins.SqlServer
{
    public class ReceiveTableDatabaseSql : ReceiveQueryFromDatabaseSql
    {
        public ReceiveTableDatabaseSql(CtorDictionary dict) : base(dict)
        {

            this.Name = nameof(ReceiveTableDatabaseSql);
            
        }
        public ReceiveTableDatabaseSql(string connectionString, string nameTable) : base(connectionString, $"select * from {nameTable}")
        {

            this.Name = nameof(ReceiveTableDatabaseSql);
        }
    }
}