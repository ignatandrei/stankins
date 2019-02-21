using StankinsCommon;
using System;
using System.Data.SqlClient;

namespace Stankins.SqlServer
{
    public class ReceiveTableDatabaseSql : ReceiveQueryFromDatabaseSql
    {
        protected string nameTable;
        public ReceiveTableDatabaseSql(CtorDictionary dict) : base(
            new CtorDictionary( dict).AddMyValue(nameof(sql),
                dict.ContainsKey(nameof(nameTable))? 
                $"select * from {dict[nameof(nameTable)]}":
                throw new ArgumentException($"missing nameof(nameTable)",nameof(nameTable))))
        {
            Name = nameof(ReceiveTableDatabaseSql);
           // this.nameTable=GetMyDataOrThrow<string>(nameTable);
        }

        public ReceiveTableDatabaseSql(string connectionString, string nameTable) :
            this(
            new CtorDictionary()
            {
                {nameof(connectionString), connectionString},
                {nameof(nameTable),nameTable}

            })

        {
            
        }
    }
}