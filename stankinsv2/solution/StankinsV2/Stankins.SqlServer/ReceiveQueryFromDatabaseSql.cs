using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;

namespace Stankins.SqlServer
{
    public class ReceiveQueryFromDatabaseSql : DatabaseReceiver
    {
        private readonly string sql;

        public ReceiveQueryFromDatabaseSql(CtorDictionary dict) : base(dict)
        {
            this.sql = GetMyDataOrThrow<string>(nameof(sql));
        }
        public ReceiveQueryFromDatabaseSql(string connectionString, string sql) : base(connectionString, typeof(SqlConnection).FullName)
        {
            this.sql = sql;
            base.dataNeeded.Add(nameof(sql),sql);
        }
        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
                receiveData=new DataToSentTable();

            var dt = await FromSql(sql);
            var arr= FastAddTables(receiveData, dt).ToArray();
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}