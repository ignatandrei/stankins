using ReceiverDB;
using System;
using System.Data.SqlClient;
namespace ReceiverDBSqlServer
{
    public class ReceiverTableSQLServer<T> : ReceiverTable<T, SqlConnection>
        where T : IEquatable<T>
    {
        public ReceiverTableSQLServer(DBTableData<T, SqlConnection> dtd) : base(dtd)
        {
        }
    }
}
