using Microsoft.Data.Sqlite;
using ReceiverDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using StankinsInterfaces;

namespace StankinsTests
{
    public class DBTableDataSqliteMemory<T>:DBTableData<T, SqliteConnection>
        where T:IComparable<T>
    {
        SqliteConnection cn;
        public DBTableDataSqliteMemory(SqliteConnection cn, ISerializeData data) :base(data)
        {
            this.cn = cn;
        }
        public override async Task<DbConnection> GetConnection()
        {
            return cn;
        }
    }
}
