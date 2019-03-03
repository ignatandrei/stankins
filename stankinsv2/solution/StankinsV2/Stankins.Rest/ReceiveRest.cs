using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Rest
{
    public class ReceiveRest: BaseObject,IReceive
    {
        private string adress;
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
        public ReceiveRest (CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiveRest);
            adress = GetMyDataOrThrow<string>(nameof(adress));

        }
       
        public ReceiveRest(string adress) : this(new CtorDictionary()
        {
            {nameof(adress),adress },
          

        })
        {

        }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData=new DataToSentTable();
            }
             var file = new ReadFileToString
            {
                FileEnconding = Encoding.UTF8,
                FileToRead = this.adress
            };
            var data = await file.LoadData();
            var dt = FromJSon(data).ToArray();
            FastAddTables(receiveData,dt);
            return receiveData;

        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
