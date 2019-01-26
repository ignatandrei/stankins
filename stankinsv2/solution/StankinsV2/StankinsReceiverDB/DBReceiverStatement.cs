using System.Data;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace StankinsReceiverDB
{
    public class DBReceiverStatement : DatabaseReceiver
    {
        protected readonly string stmtSql;
        public DBReceiverStatement(CtorDictionary dict) : base(dict)
        {
            stmtSql = GetMyDataOrThrow<string>(nameof(stmtSql));
        }
        public DBReceiverStatement(string connectionString, string connectionType, string sql) : base(connectionString,connectionType)
        {
            this.stmtSql = sql;
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {

            if (receiveData == null)
            {
                receiveData = new DataToSentTable();

            }
            using (var cn = NewConnection())
            {
                await cn.OpenAsync();
                using(var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = stmtSql;
                    cmd.CommandType = CommandType.Text;
                    using(var ir=await cmd.ExecuteReaderAsync())
                    {
                        var dt = new DataTable();
                        dt.Load(ir);
                        var id = receiveData.AddNewTable(dt);
                        receiveData.Metadata.AddTable(dt, id);
                       

                    }
                }
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
