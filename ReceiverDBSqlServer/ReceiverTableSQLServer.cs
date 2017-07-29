using ReceiverDB;
using System;
using System.Data.SqlClient;
namespace ReceiverDBSqlServer
{
    public abstract class ReceiverTableSQLServer<T> : ReceiverTable<T, SqlConnection>
        where T : IEquatable<T>
    {
        public  ReceiverTableSQLServer(DBTableData<T, SqlConnection> dtd) : base(dtd)
        {
        }
        /// <summary>
        /// TODO: what if not sql server 2016?
        /// </summary>
        /// <returns></returns>
        protected override string MaxRowsToLoad()
        {
            if (tableData.MaxRecordsToRead == long.MaxValue)
                return "";

            string pag = "";
            //if (tableData.PageNumber > 1)
            //{
            //    var rows = (tableData.PageNumber-1) * tableData.MaxRecordsToRead;
            //    pag += $" OFFSET {rows}";
            //}
            pag += $" FETCH NEXT {tableData.MaxRecordsToRead} ";
            return pag;
        }
    }
}
