using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Stankins.Rest
{
    public class SenderJSONPOST : BaseObject, ISender
    {
        private string adress;
        private string tableName;
        public SenderJSONPOST(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            Name = nameof(ReceiveRestFromFile);
            this.tableName = GetMyDataOrDefault<string>(nameof(tableName),"");
            this.adress= GetMyDataOrThrow<string>(nameof(adress));

        }

        public SenderJSONPOST(string adress, string tableName=null) : this(new CtorDictionary()
        {
            {nameof(adress),adress },
            {nameof(tableName),tableName},


        })
        {

        }
        

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            DataTable dt;
            if (string.IsNullOrWhiteSpace(tableName) && receiveData.DataToBeSentFurther.Count==1)
            {
                dt = receiveData.DataToBeSentFurther.First().Value;
            }
            else
            {
                dt=receiveData.FindAfterName(tableName).Value;
            }
            var rows= JArray.FromObject(dt).Select(it => it.ToString(Formatting.None)).ToArray();
            SendDataWeb send =new SendDataWeb();
            foreach (string item in rows)
            {
                var res=await send.PostJSON(this.adress,item);
                
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
