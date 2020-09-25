using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Stankins.SqlServer
{
    public class ReceiveMetadataFromDatabaseSql : DatabaseReceiver
    {
        public ReceiveMetadataFromDatabaseSql(CtorDictionary dataNeeded) : base(new CtorDictionary(dataNeeded)
            .AddMyValue(
                nameof(connectionType), typeof(SqlConnection).AssemblyQualifiedName)
            )
        {
            this.Name = nameof(ReceiveMetadataFromDatabaseSql);
        }
        public ReceiveMetadataFromDatabaseSql(string connectionString) : this(
            new CtorDictionary(){
              {nameof(  connectionString),connectionString }
            })
        {
            this.Name = nameof(ReceiveMetadataFromDatabaseSql);
        }
        
        
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var tablesProperties =new DataTable("properties");
           
            var tablesString = $@"select t.object_id as id, s.name +'.'+ t.name as name from sys.tables t
                            inner join sys.schemas s on t.schema_id = s.schema_id order by 2 ";
            var newTables = FromSql(tablesString,"tables");


            var viewsString = $@"select t.object_id as id, s.name +'.'+ t.name as name from sys.views t
                            inner join sys.schemas s on t.schema_id = s.schema_id order by 2 ";
            var newViews = FromSql(viewsString, "views");

            var cols = $@"select cast(c.column_id as nvarchar) +'_'+ cast(c.object_id as varchar) as id, c.name,c.object_id as tableId,t.name as type  
                        
,case when coalesce(c.is_nullable,0) = 1 then 1
else 0
end as IS_NULLABLE
                        from sys.columns c
                        inner join sys.types t on t.system_type_id = c.system_type_id
           left join sys.tables o on o.object_id = c.object_id
						left join sys.views vw on vw.object_id = c.object_id

where len(coalesce(cast(o.object_id as varchar),cast(vw.object_id as varchar),''))>0              
order by 2";
      
            var newCols =FromSql(cols, "columns");
            var rels = $@"select 

a.object_id as id , a.name,

b.parent_object_id,cast(b.parent_column_id as nvarchar)+'_'+ cast(b.parent_object_id as nvarchar) as parent_column_id,
b.referenced_object_id,cast(b.referenced_column_id as nvarchar) + '_'+ cast(b.referenced_object_id as nvarchar) as referenced_column_id


from sys.foreign_keys a
    join sys.foreign_key_columns b
                on a.object_id=b.constraint_object_id order by 2";
            var newRels = FromSql(rels, "relations");
            


            var keySql = @"SELECT
DISTINCT
OBJECT_ID(Table_Schema +'.'+ Constraint_Name) as id,
Table_Schema +'.'+ Constraint_Name AS name,
OBJECT_ID(Table_Schema +'.'+ TABLE_NAME) as tableId,
cast(c.column_id as varchar)+'_'+ cast(OBJECT_ID(Table_Schema +'.'+ TABLE_NAME) as varchar) as column_id,
o.type_desc
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
inner join sys.objects o on o.object_id = OBJECT_ID(Table_Schema +'.'+ Constraint_Name)
inner join sys.columns c on OBJECT_ID(Table_Schema +'.'+ TABLE_NAME) = c.object_id
and c.name =COLUMN_NAME";

            var newKeys = FromSql(keySql,"keys");
            await Task.WhenAll(newTables, newCols, newRels, newKeys, newViews);

            
            

            var props=
                "select t.object_id as id, 'tables' as TableName,t.* from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id";

            var propsNew=await FromSqlToProperties(props);
            tablesProperties.Merge(propsNew,true,MissingSchemaAction.Add);


            props =
                "select cast(c.column_id as nvarchar) +'_'+ cast(c.object_id as varchar) as id, 'columns' as TableName,c.* from sys.columns c inner join sys.tables o on o.object_id = c.object_id";

            propsNew = await FromSqlToProperties(props);
            tablesProperties.Merge(propsNew, true, MissingSchemaAction.Add);

            props = @"SELECT 
cast(c.column_id as nvarchar) +'_'+ cast(c.object_id as varchar) as id, 
 'columns' as TableName,

schemacols.* FROM INFORMATION_SCHEMA.COLUMNS schemaCols
inner join sys.columns c on schemaCols.COLUMN_NAME = c.name
inner join sys.tables t on t.object_id = c.object_id and t.name = schemaCols.TABLE_NAME
inner join sys.schemas s on t.schema_id = s.schema_id and s.name = schemaCols.TABLE_SCHEMA
";
            propsNew = await FromSqlToProperties(props);
            tablesProperties.Merge(propsNew, true, MissingSchemaAction.Add);


            var ids=FastAddTables(receiveData, newTables.Result,newCols.Result,newRels.Result, newKeys.Result,tablesProperties, newViews.Result);
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
