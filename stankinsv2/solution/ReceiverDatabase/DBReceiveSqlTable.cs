using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ReceiverDatabase
{
    public class DBReceiveSqlTable<Connection> : DBReceiveSql<Connection>
        where Connection : DbConnection, new()
    {
        public DBReceiveSqlTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }
        public DBReceiveSqlTable(string connectionString, string nameTable)
            : base(connectionString, CommandType.TableDirect, nameTable)
        {

        }
        public override async Task<IMetadata> LoadMetadata()
        {
            using (var con = new Connection())
            {
                con.ConnectionString = ConnectionString;
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"select * from {CommandText} where 1=0" ;
                    cmd.CommandType = this.CommandType;
                    using (var ir = await cmd.ExecuteReaderAsync())
                    {
                        #region gather data
                        var dt = new DataTable
                        {
                            TableName = this.Name
                        };
                        dt.Load(ir);
                        #endregion

                        #region gather metadata
                        var ret = new DataToSentTable();
                        var id= ret.AddNewTable(dt);

                        
                        var idTable = ret.Metadata.AddTable(dt,id);
                        
                        #endregion
                        return ret.Metadata;
                    }
                }
            }
        }
    }
}
