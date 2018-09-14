using ReceiverDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ReceiverDBSqlServer
{
    public class ReceiverTableSQLServerDateTime : ReceiverTableSQLServer<DateTime>
    {
        public ReceiverTableSQLServerDateTime(DBTableData<DateTime, SqlConnection> dtd) : base(dtd)
        {
        }
    }
}
