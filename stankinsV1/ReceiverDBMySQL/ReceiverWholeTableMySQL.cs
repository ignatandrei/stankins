using MySql.Data.MySqlClient;
using ReceiverDB;
using StanskinsImplementation;

namespace ReceiverDBMySQL
{
    public class ReceiverWholeTableMySQL : ReceiverTableMySQL<FakeComparable>
    {

        public ReceiverWholeTableMySQL(DBTableData<FakeComparable, MySqlConnection> dtd) : base(dtd)
        {
            dtd.FieldNameToMark = "";
        }
    }
}
