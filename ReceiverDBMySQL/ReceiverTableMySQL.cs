using MySql.Data.MySqlClient;
using ReceiverDB;
using System;

namespace ReceiverDBMySQL
{
    public abstract class ReceiverTableMySQL<T> : ReceiverTable<T, MySqlConnection>
    where T : IComparable<T>
    {
        public ReceiverTableMySQL(DBTableData<T, MySqlConnection> dtd) : base(dtd)
        {
        }
    }
}
