using Stankins.Interfaces;
using StankinsCommon;
using StankinsReceiverDB;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Stankins.SqlServer
{
    public class ReceiveQueryFromDatabaseSql : DBReceiverStatement
    {
        
        public ReceiveQueryFromDatabaseSql(CtorDictionary dict) : base(
            new CtorDictionary(dict)
            .AddMyValue(
                nameof(connectionType), typeof(SqlConnection).AssemblyQualifiedName)
            )
        {
            Name = nameof(ReceiveQueryFromDatabaseSql);
        }

        public ReceiveQueryFromDatabaseSql(string connectionString, string sql) : this(
            new CtorDictionary()
            {
                {nameof(connectionString), connectionString},
                {nameof(sql),sql}

            })
        {

        }
       



        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}