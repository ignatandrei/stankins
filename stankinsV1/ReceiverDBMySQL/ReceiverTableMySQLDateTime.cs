using MySql.Data.MySqlClient;
using ReceiverDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiverDBMySQL
{
    public class ReceiverTableMySQLDateTime : ReceiverTableMySQL<DateTime>
    {
        public ReceiverTableMySQLDateTime(DBTableData<DateTime, MySqlConnection> dtd) : base(dtd)
        {
        }
    }
}
