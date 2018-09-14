using ReceiverDB;
using StanskinsImplementation;
using System.Data.SqlClient;
namespace ReceiverDBSqlServer
{
    public class ReceiverWholeTableSqlServer: ReceiverTableSQLServer<FakeComparable>
    {

        public ReceiverWholeTableSqlServer(DBTableData<FakeComparable, SqlConnection> dtd) : base(dtd)
        {
            dtd.FieldNameToMark = "";
        }
    }
}
