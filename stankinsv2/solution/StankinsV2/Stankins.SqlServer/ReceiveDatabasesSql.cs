﻿using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using StankinsReceiverDB;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Stankins.SqlServer
{
    public class ReceiveDatabasesSql : DatabaseReceiver
    {
        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }
        public ReceiveDatabasesSql(CtorDictionary dict) : base(dict)
        {
            this.Name = nameof(ReceiveDatabasesSql);
        }
        public ReceiveDatabasesSql(string connectionString) : this(
            new CtorDictionary()
            {
                {nameof(connectionString),connectionString },
                {nameof(connectionType), typeof(SqlConnection).AssemblyQualifiedName}
            }
            )
        {
            this.Name = nameof(ReceiveDatabasesSql);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var databasesString = "select database_id as id ,name from sys.databases";//2008+
            var databases = await FromSql(databasesString,"databases");
            databases.TableName = "databases";
            FastAddTables(receiveData, databases);
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
