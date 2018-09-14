using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using ReceiverDB;
using ReceiverDBMySQL;
using ReceiverDBSQLite;
using ReceiverDBSqlServer;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BlocklyClasses
{
    public enum ReceiverDBType
    {
        None=0,
        SqlServer=1,
        SqlLite=2,
        MySql=3
    }
    public class ReceiverWholeTable : IReceive
    {
        public ReceiverDBType ReceiverType { get; set; }
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public IRowReceive[] valuesRead { get; set; }

        public string Name { get; set; }

        public void ClearValues()
        {
            valuesRead = null;
        }

        public async Task LoadData()
        {
            IReceive r;
            switch(this.ReceiverType)
            {
                case ReceiverDBType.SqlServer:
                    var dtdSql = new DBTableData<FakeComparable, SqlConnection>(new SerializeDataInMemory())
                    {
                        ConnectionString = ConnectionString,
                        TableName = TableName
                    };
                    r = new ReceiverWholeTableSqlServer(dtdSql);
                    break;
                case ReceiverDBType.MySql:
                    var dtdMySql = new DBTableData<FakeComparable, MySqlConnection>(new SerializeDataInMemory())
                    {
                        ConnectionString = ConnectionString,
                        TableName = TableName
                    };
                    r = new ReceiverWholeTableMySQL(dtdMySql);
                    break;
                case ReceiverDBType.SqlLite:
                    var dtdSqlLite = new DBTableData<FakeComparable, SqliteConnection>(new SerializeDataInMemory())
                    {
                        ConnectionString = ConnectionString,
                        TableName = TableName
                    };
                    r = new ReceiverWholeTableSQLite(dtdSqlLite);
                    break;

                default:
                    throw new ArgumentException($"not found receiver type {this.ReceiverType}");
            }
            
            await r.LoadData();
            valuesRead = r.valuesRead;
        }
    }
}
