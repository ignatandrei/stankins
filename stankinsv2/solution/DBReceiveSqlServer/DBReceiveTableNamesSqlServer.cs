using ReceiverDatabase;
using System;
using System.Data.Common;
using System.Data;
using StankinsV2Interfaces;
using System.Threading.Tasks;
using StankinsV2Objects;
using System.Data.SqlClient;

namespace DBReceiveSqlServer
{
    
    public class DBReceiveTableNamesSqlServer: DBReceiveSql<SqlConnection>
    {
        public DBReceiveTableNamesSqlServer(string connectionString):
            base(connectionString,CommandType.Text,"select name,object_id as id from sys.tables")
        {

        }
        public override async Task<IMetadata> LoadMetadata()
        {
            
            var m = new MetadataTable();
            //var id = m.AddTable(this,0);
            //m.AddColumn("name",id);//those are in sql
            //m.AddColumn("id", id);//those are in sql
            //TODO: use valuetask
            return await Task.FromResult(m);
        }
    }
}
