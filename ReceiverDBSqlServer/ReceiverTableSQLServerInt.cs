using ReceiverDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ReceiverDBSqlServer
{
    public class ReceiverTableSQLServerInt: ReceiverTableSQLServer<int>
    {
        public ReceiverTableSQLServerInt(DBTableData<int, SqlConnection> dtd) : base(dtd)
        {
        }
    }
}
