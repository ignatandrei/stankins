using ReceiverDB;
using System;
using Microsoft.Data.Sqlite;
using StankinsInterfaces;

namespace ReceiverDBSQLite
{
    public class ReceiverTableSQLite<T> : ReceiverTable<T, SqliteConnection>
        where T:IEquatable<T>
    {
        public ReceiverTableSQLite(DBTableData<T,SqliteConnection> dtd) : base(dtd)
        {
        }
    }
}
