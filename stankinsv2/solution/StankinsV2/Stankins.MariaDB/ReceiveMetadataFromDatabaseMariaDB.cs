using MySqlConnector;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Stankins.MariaDB
{
    public class ReceiveMetadataFromDatabaseMariaDB : DatabaseReceiver
    {
        public ReceiveMetadataFromDatabaseMariaDB(CtorDictionary dataNeeded) : base(new CtorDictionary(dataNeeded)
            .AddMyValue(
                nameof(connectionType), typeof(MySqlConnection).AssemblyQualifiedName)
            )
        {
            this.Name = nameof(ReceiveMetadataFromDatabaseMariaDB);
        }
        public ReceiveMetadataFromDatabaseMariaDB(string connectionString) : this(
            new CtorDictionary(){
              {nameof(  connectionString),connectionString }
            })
        {
            this.Name = nameof(ReceiveMetadataFromDatabaseMariaDB);
        }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            receiveData ??= new DataToSentTable();
            var b = new MySqlConnectionStringBuilder(base.connectionString);
            var tablesString = $"select Concat(Table_SCHEMA,'.' ,TABLE_NAME) as id , TABLE_NAME  as name from  information_schema.TABLES where TABLE_SCHEMA='{b.Database}'";
            var newTables = FromSql(tablesString, "tables");

            var cols = $@"select Concat(Table_SCHEMA,'.' ,TABLE_NAME,'.',COLUMN_NAME) as id, 
COLUMN_NAME as name, Concat(Table_SCHEMA, '.', TABLE_NAME) as tableId
, COLUMN_TYPE as type
from information_schema.COLUMNS where TABLE_SCHEMA = '{b.Database}'
";

            var newCols = FromSql(cols, "columns");
            var rels = $@"select CONCAT(Table_SCHEMA,'.' ,TABLE_NAME,'.',COLUMN_NAME,'.',CONSTRAINT_NAME) as id

,CONSTRAINT_NAME as name
,CONCAT(Table_SCHEMA,'.',TABLE_NAME) as parent_object_id
,CONCAT(Table_SCHEMA,'.',TABLE_NAME,'.',COLUMN_NAME) as parent_column_id
,CONCAT(REFERENCED_TABLE_SCHEMA,'.',REFERENCED_TABLE_NAME) as referenced_object_id
,CONCAT(REFERENCED_TABLE_SCHEMA,'.',REFERENCED_TABLE_NAME,'.',REFERENCED_COLUMN_NAME) as referenced_column_id

from information_schema.KEY_COLUMN_USAGE  where TABLE_SCHEMA='{b.Database}'
";
            var newRels = FromSql(rels, "relations");


            var keySql = @"SELECT
DISTINCT
CONCAT(Table_Schema ,'.',TABLE_NAME,'.',COLUMN_NAME,'.',Constraint_Name) as id,
CONCAT(Table_Schema ,'.',Constraint_Name ) AS name,
CONCAT(Table_Schema ,'.', TABLE_NAME) as tableId,
CONCAT(Table_SCHEMA ,'.', TABLE_NAME,'.',COLUMN_NAME) as column_id,
'PRIMARY' as type_desc
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
;
            keySql += $@"where TABLE_SCHEMA='{b.Database}'";
            keySql += $@"and Constraint_Name='PRIMARY'";

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
