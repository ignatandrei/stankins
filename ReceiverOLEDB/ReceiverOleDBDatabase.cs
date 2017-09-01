using ReceiverDB;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverOLEDB
{
    public class ReceiverOleDBDatabaseInt: ReceiverOleDBDatabase<int>
    {
        public ReceiverOleDBDatabaseInt(DBTableData<int, OleDbConnection> dtd):base(dtd)
        {

        }
    }
    public class ReceiverOleDBDatabase<T> : ReceiverTable<T, OleDbConnection>
         where T : IComparable<T>         
    {
        public ReceiverOleDBDatabase(DBTableData<T, OleDbConnection> dtd):base(dtd)
        {
            
        }
    }
}
