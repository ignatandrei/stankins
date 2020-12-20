
using Npgsql;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Data.Common;
using System.Threading.Tasks;
namespace Stankins.Postgresql
{
    public class ReceiveMetadataFromDatabasePostGresSql : DatabaseReceiver
    {
        public ReceiveMetadataFromDatabasePostGresSql(CtorDictionary dataNeeded) : base(new CtorDictionary(dataNeeded)
            .AddMyValue(
                nameof(connectionType), typeof(NpgsqlConnection).AssemblyQualifiedName)
            )
        {
            this.Name = nameof(ReceiveMetadataFromDatabasePostGresSql);
        }
        public ReceiveMetadataFromDatabasePostGresSql(string connectionString) : this(
            new CtorDictionary(){
              {nameof(  connectionString),connectionString }
            })
        {
            this.Name = nameof(ReceiveMetadataFromDatabasePostGresSql);
        }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            receiveData ??= new DataToSentTable();
            var b = new NpgsqlConnectionStringBuilder(base.connectionString);
            var tablesString = $"select Concat(Table_SCHEMA,'.' ,TABLE_NAME) as id , TABLE_NAME  as name from  information_schema.TABLES where TABLE_Catalog='{b.Database}'";
            var newTables = FromSql(tablesString, "tables");

            var cols = $@"select 
case when COALESCE (IS_NULLABLE , '') = 'YES' then 1
else 0
end as IS_NULLABLE,
Concat(Table_SCHEMA,'.' ,TABLE_NAME,'.',COLUMN_NAME) as id, 
COLUMN_NAME as name, Concat(Table_SCHEMA, '.', TABLE_NAME) as tableId
, Data_type as type
from information_schema.COLUMNS where TABLE_Catalog = '{b.Database}'
";

            var newCols = FromSql(cols, "columns");
            var rels = $@"

SELECT
CONCAT(tc.Table_SCHEMA,'.' ,tc.TABLE_NAME,'.',kcu.COLUMN_NAME,'.',tc.CONSTRAINT_NAME) as id,
tc.CONSTRAINT_NAME as NAME,
CONCAT(tc.Table_SCHEMA,'.',tc.TABLE_NAME) as parent_object_id,
CONCAT(tc.Table_SCHEMA,'.',tc.TABLE_NAME,'.',kcu.COLUMN_NAME) as parent_column_id,
CONCAT(ccu.table_schema ,'.',ccu.table_name )as referenced_object_id,
CONCAT(ccu.table_schema ,'.',ccu.table_name ,'.',ccu.column_name) as referenced_column_id
   
FROM 
    information_schema.table_constraints AS tc 
    JOIN information_schema.key_column_usage AS kcu
      ON tc.constraint_name = kcu.constraint_name
      AND tc.table_schema = kcu.table_schema
    JOIN information_schema.constraint_column_usage AS ccu
      ON ccu.constraint_name = tc.constraint_name
      AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'


and TABLE_Catalog='{b.Database}'
";
            var newRels = FromSql(rels, "relations");


            var keySql = @"SELECT 
CONCAT(tc.Table_Schema ,'.',tc.TABLE_NAME,'.',c.COLUMN_NAME,'.',Constraint_Name) as id,
CONCAT(tc.Table_Schema ,'.',Constraint_Name ) AS name,
CONCAT(tc.Table_Schema ,'.', tc.TABLE_NAME) as tableId,
CONCAT(tc.Table_SCHEMA ,'.', tc.TABLE_NAME,'.',c.COLUMN_NAME) as column_id,
'PRIMARY' as type_desc

FROM information_schema.table_constraints tc 
JOIN information_schema.constraint_column_usage AS ccu USING (constraint_schema, constraint_name) 
JOIN information_schema.columns AS c ON c.table_schema = tc.constraint_schema
  AND tc.table_name = c.table_name AND ccu.column_name = c.column_name "
;
            keySql += $@"where TABLE_Catalog='{b.Database}'";
            keySql += $@"and constraint_type='PRIMARY KEY'";

            var newKeys = FromSql(keySql, "keys");

            await Task.WhenAll(newTables, newCols, newRels, newKeys);
            var ids = FastAddTables(receiveData, newTables.Result, newCols.Result, newRels.Result, newKeys.Result);
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
