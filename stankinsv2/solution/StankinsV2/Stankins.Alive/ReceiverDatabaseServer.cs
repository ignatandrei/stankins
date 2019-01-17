using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverDBServer : AliveStatus, IReceive
    {
        protected readonly string connectionString;
        protected readonly string connectionType;

        public ReceiverDBServer(CtorDictionary dict) : base(dict)
        {
            connectionString = GetMyDataOrThrow<string>(nameof(connectionString));
            connectionType = GetMyDataOrThrow<string>(nameof(connectionType));
        }
        public ReceiverDBServer(string connectionString,string connectionType) : this(new CtorDictionary()
        {
            {nameof(connectionString),connectionString },
            {nameof(connectionType),connectionType }
        })
        {
          
        }
        protected virtual DbConnection NewConnection()
        {
            return Activator.CreateInstance(Type.GetType(connectionType)) as DbConnection;
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();
            DataTable results = CreateTable();
            var sw = Stopwatch.StartNew();
            var StartedDate = DateTime.UtcNow;
            try
            {
                
                var resp = NewConnection();
                using (resp)
                {
                    resp.ConnectionString = connectionString;
                    await resp.OpenAsync();

                    results.Rows.Add("receiverdatabaseserver", "", connectionString, true, resp.State, sw.ElapsedMilliseconds, resp.ServerVersion, null,StartedDate);

                }
            }
            catch (Exception ex)
            {
                results.Rows.Add("receiverdatabaseserver", "", connectionString, false, null, sw.ElapsedMilliseconds, null, ex.Message,StartedDate);
            }
            receiveData.AddNewTable(results);
            receiveData.Metadata.AddTable(results, receiveData.Metadata.Tables.Count);

            return await Task.FromResult(receiveData) ;
        }
    }
    public class ReceiverDBServer<T> : ReceiverDBServer
        where T : DbConnection, new()
    {
        
        public ReceiverDBServer(CtorDictionary dict) : base(dict)
        {
        }
        public ReceiverDBServer(string connectionString) : this(new CtorDictionary()
        {
            {nameof(connectionString),connectionString },
            {nameof(connectionType),typeof(T).ToString() }
        })
        {

        }
        protected override DbConnection NewConnection()
        {
            return new T();
        }
        
    }

    public class ReceiverDBSqlServer : ReceiverDBServer<SqlConnection>
    {
        public ReceiverDBSqlServer(CtorDictionary dict) : base(dict)
        {
           
        }
        public ReceiverDBSqlServer(string connectionString) : base(connectionString)
        {

        }
    }
}
