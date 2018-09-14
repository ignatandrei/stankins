using Microsoft.Data.Sqlite;
using ReceiverDB;
using StanskinsImplementation;

namespace ReceiverDBSQLite
{
    public class ReceiverWholeTableSQLite : ReceiverTableSQLite<FakeComparable>
    {

        public ReceiverWholeTableSQLite(DBTableData<FakeComparable, SqliteConnection> dtd) : base(dtd)
        {
            dtd.FieldNameToMark = "";
        }
    }
}
