using System.Data;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace StankinsReceiverDB
{
    public class DBReceiverStatement : DatabaseReceiver
    {
        protected readonly string sql;
        public DBReceiverStatement(CtorDictionary dict) : base(dict)
        {
            sql = GetMyDataOrThrow<string>(nameof(sql));
            this.Name = nameof(DBReceiverStatement);
        }
        public DBReceiverStatement(string connectionString, string connectionType, string sql) : this(
            new CtorDictionary()
            {
                {nameof(connectionString),connectionString},
                {nameof(connectionType),connectionType},
                {nameof(sql),sql}
            })
        {
        
            
        }
        
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {

            if (receiveData == null)
            {
                receiveData = new DataToSentTable();

            }
            var dt = await FromSql(sql,"");
            FastAddTable(receiveData,dt);
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
