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
     
        public ReceiveQueryFromDatabaseSql(CtorDictionary dict) : base(dict)
        {           
            this.Name = nameof(ReceiveQueryFromDatabaseSql);
        }
        
        public ReceiveQueryFromDatabaseSql(string connectionString, string sql) : this(
            new CtorDictionary()
            {
                {nameof(connectionString), connectionString},
                {nameof(connectionType), typeof(SqlConnection).FullName},
                {nameof(sql),sql}

            }            )
        {
           
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