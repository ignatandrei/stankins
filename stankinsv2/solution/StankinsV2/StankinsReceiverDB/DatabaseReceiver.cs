using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data.Common;

namespace StankinsReceiverDB
{
    public abstract class DatabaseReceiver: BaseObject, IReceive
    {
        protected readonly string connectionString;
        protected readonly string connectionType;

        
        public DatabaseReceiver(CtorDictionary dict) : base(dict)
        {
            connectionString = GetMyDataOrThrow<string>(nameof(connectionString));
            connectionType = GetMyDataOrThrow<string>(nameof(connectionType));
        }
        public DatabaseReceiver(string connectionString, string connectionType) : this(new CtorDictionary()
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

    }
}
