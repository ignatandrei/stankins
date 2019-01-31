using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
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
            var tablesProperties =new DataTable("properties");
            var tables = new DataTable();
            tables.TableName = "tables";
            var columns = new DataTable();
            columns.TableName = "columns";
            var relations = new DataTable();
            relations.TableName = "relations";
            //tables.Columns.Add(new DataColumn("id", typeof(long)));
            //tables.Columns.Add(new DataColumn("name", typeof(string)));
            //tables.Columns.Add(new DataColumn("databaseId", typeof(long)));
           
            var tablesString = $@"select t.object_id as id, s.name +'.'+ t.name as name from sys.tables t
                            inner join sys.schemas s on t.schema_id = s.schema_id order by 2 ";
            var newTables = await FromSql(tablesString);
            tables.Merge(newTables, true, MissingSchemaAction.Add);
            var cols = $@"select c.column_id as id, c.name,c.object_id as tableId  
                        from sys.columns c
                        inner join sys.tables o on o.object_id = c.object_id order by 2";
            var newCols =await FromSql(cols);
            columns.Merge(newCols, true, MissingSchemaAction.Add);
            var rels = $@"select a.object_id as id , a.name,b.parent_object_id,b.parent_column_id,b.referenced_object_id,b.referenced_column_id
from sys.foreign_keys a
    join sys.foreign_key_columns b
                on a.object_id=b.constraint_object_id order by 2";
            var newRels = await FromSql(rels);
            relations.Merge(newRels, true, MissingSchemaAction.Add);


            var props=
                "select t.object_id as id, 'tables' as TableName,t.* from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id";

            var tablesPropertiesNew=await FromSqlToProperties(props);
            tablesProperties.Merge(tablesPropertiesNew,true,MissingSchemaAction.Add);


            props =
                "select c.column_id as id, 'columns' as TableName,c.* from sys.columns c inner join sys.tables o on o.object_id = c.object_id";

            tablesPropertiesNew = await FromSqlToProperties(props);
            tablesProperties.Merge(tablesPropertiesNew, true, MissingSchemaAction.Add);



            var ids=FastAddTables(receiveData, tables,columns,relations, tablesProperties).ToArray();
            var r = new Relation();
            r.IdTableParent = ids[0];
            r.IdTableChild = ids[1];
            r.ColumnParent = "id";
            r.ColumnChild = "databaseId";
            receiveData.Metadata.Relations.Add(r);

            r = new Relation();
            r.IdTableParent = ids[1];
            r.IdTableChild = ids[2];
            r.ColumnParent = "id";
            r.ColumnChild = "tableId";
            receiveData.Metadata.Relations.Add(r);



            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
