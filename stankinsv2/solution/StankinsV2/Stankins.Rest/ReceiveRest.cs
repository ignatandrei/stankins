using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Rest
{
    public abstract class ReceiveRest: BaseObject,IReceive
    {
        public ReceiveRest(CtorDictionary dataNeeded) : base(dataNeeded)
        {
        }

        protected virtual IEnumerable<DataTable> FromJSon(string json)
        {
            var token=JToken.Parse(json);
            if (!(token is JArray))
            {
                json = "[" + json + "]";
                token=JToken.Parse(json);
                
            }
            JArray tok=token as JArray;
         
            yield return token.ToObject<DataTable>();
           


        }
        public abstract Task<string> GetData();        

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData=new DataToSentTable();
            }
            var dt = FromJSon(await GetData()).ToArray();
            FastAddTables(receiveData,dt);
            return receiveData;

        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
