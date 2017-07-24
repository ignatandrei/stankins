using Microsoft.Data.Sqlite;
using ReceiverDB;
using StankinsInterfaces;

namespace ReceiverDBSQLite
{
    public class ReceiverTableSQLiteInt : ReceiverTableSQLite<int>      
    {
        public ReceiverTableSQLiteInt(DBTableData<int, SqliteConnection> dtd) : base(dtd)
        {
        }
    }
}
