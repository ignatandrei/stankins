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
    public class ReceiveQueryFromDatabaseSql : DBReceiverStatement 
    {
        private readonly string sql;

        public ReceiveQueryFromDatabaseSql(CtorDictionary dict) : base(dict)
        {
           
            this.Name = nameof(ReceiveQueryFromDatabaseSql);
        }
        public ReceiveQueryFromDatabaseSql(string connectionString, string sql) : base(connectionString, typeof(SqlConnection).FullName,sql)
        {
           
            this.Name = nameof(ReceiveQueryFromDatabaseSql);
        }
        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }

        

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}