using ReceiverDB;
using System;
using Microsoft.Data.Sqlite;
using StankinsInterfaces;

namespace ReceiverDBSQLite
{
    public abstract class ReceiverTableSQLite<T> : ReceiverTable<T, SqliteConnection>
        where T:IEquatable<T>
    {
        public ReceiverTableSQLite(DBTableData<T,SqliteConnection> dtd) : base(dtd)
        {
        }
        protected override string MaxRowsToLoad()
        {
            if (tableData.MaxRecordsToRead == long.MaxValue)
                return "";

            string pag = "";
            
            pag += $" LIMIT {tableData.MaxRecordsToRead} ";
            //if (tableData.PageNumber > 1)
            //{
            //    var rows = (tableData.PageNumber - 1) * tableData.MaxRecordsToRead;
            //    pag += $" OFFSET {rows}";
            //}
            return pag;
        }
    }
}
