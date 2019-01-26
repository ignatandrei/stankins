using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Stankins.SqlServer
{
    public class ReceiveMetadataFromDatabaseSql : DatabaseReceiver
    {
        public ReceiveMetadataFromDatabaseSql(CtorDictionary dict) : base(dict)
        {
            
        }
        public ReceiveMetadataFromDatabaseSql(string connectionString) : base(connectionString, typeof(SqlConnection).FullName)
        {
        }
        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var databasesString = "select database_id as id ,name from sys.databases";//2008+
            var databases = await FromSql(databasesString);
            databases.TableName = "databases";
            var tables = new DataTable();
            tables.TableName = "tables";
            var columns = new DataTable();
            columns.TableName = "columns";
            //tables.Columns.Add(new DataColumn("id", typeof(long)));
            //tables.Columns.Add(new DataColumn("name", typeof(string)));
            //tables.Columns.Add(new DataColumn("databaseId", typeof(long)));
            foreach (DataRow item in databases.Rows)
            {
                var db = item["name"].ToString();
                var idDb = item["id"];
                var tablesString = $@"select t.object_id as id, s.name +'.'+ t.name as name,{idDb} as databaseId from {db}.sys.tables t
                                inner join {db}.sys.schemas s on t.schema_id = s.schema_id";
                var newTables = await FromSql(tablesString);
                tables.Merge(newTables, true, MissingSchemaAction.Add);
                var cols = $@"select column_id as id, name,object_id as tableId  from sys.all_columns";
                var newCols =await FromSql(cols);
                columns.Merge(newCols, true, MissingSchemaAction.Add);
            }

            FastAddTables(receiveData,databases, tables,columns);

            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
