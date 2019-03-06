using System.Data;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class ReceiveTextToTable : Receive
    {
        private readonly string text;
        private readonly string colName;
        private readonly string tableName;

        public ReceiveTextToTable(string text,string colName=null,string tableName=null):this(new CtorDictionary()
        {
            {nameof(text),text},
            {nameof(colName),colName},
            {nameof(tableName),tableName}

        })
        {
        }
        public ReceiveTextToTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.text = GetMyDataOrThrow<string>(nameof(text));
            this.colName =GetMyDataOrDefault<string>(nameof(colName),"column1");
            this.tableName = GetMyDataOrDefault<string>(nameof(tableName),"table1");
        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var dt=new DataTable(tableName);
            dt.Columns.Add(colName,typeof(string));
            dt.Rows.Add(text);
            FastAddTable(receiveData,dt);
            return Task.FromResult(receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
