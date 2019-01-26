using Stankins.Interfaces;
using StankinsCommon;
using StankinsReceiverDB;
using System;
using System.Threading.Tasks;

namespace Stankins.SqlServer
{
    public class ReceiveMetadataFromDatabaseSql : DatabaseReceiver
    {
        public ReceiveMetadataFromDatabaseSql(CtorDictionary dict) : base(dict)
        {
            
        }
        public ReceiveMetadataFromDatabaseSql(string connectionString, string connectionType) : base(connectionString, connectionType)
        {
        }
        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            return null;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
