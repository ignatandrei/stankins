using Microsoft.Data.Sqlite;
using ReceiverDB;
using StankinsInterfaces;
using System;

namespace ReceiverDBSQLite
{
    public class ReceiverTableSQLiteDateTime : ReceiverTableSQLite<DateTime>
    {
        public ReceiverTableSQLiteDateTime(DBTableData<DateTime,SqliteConnection> dtd) : base(dtd)
        {
        }
    }
}
