using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Cachet
{
    public class SenderCachet : BaseObject, ISender
    {
        private readonly string urlCachet;
        private readonly string tokenCachet;

        public SenderCachet(string urlCachet, string tokenCachet):this(new CtorDictionary()
        {
            {nameof(tokenCachet),tokenCachet},
            {nameof(urlCachet),urlCachet}
        })
        {
           
        }
        public SenderCachet(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            tokenCachet= this.GetMyDataOrThrow<string>(nameof(tokenCachet));
            urlCachet = this.GetMyDataOrThrow<string>(nameof(urlCachet));
        }
        async Task<List<KeyValuePair<long,string>>> components()
        {
            var ret= new List<KeyValuePair<long,string>>();
            //TODO: replace with json stankins
            //TODO: replace with another component that does better cachet
            var f=new ReadFileToString();
            f.FileToRead=urlCachet +"/api/v1/components";
            var data=await f.LoadData();
            var token=  JToken.Parse(data);
            var comps= token["data"]as JArray;
            foreach(var item in comps)
            {
                int id=int.Parse(item["id"].ToString());
                ret.Add(new KeyValuePair<long, string>(id,item["name"].ToString()));
            }
            return ret;
        }
        private async Task CreateNewIncident(long componentId, int componentStatus,string incidentName, string incidentMessage, int incidentStatus)
        {
            var url=urlCachet +"/api/v1/incidents";
            var obj=new{
            name = incidentName,
            message=incidentMessage,
            status =incidentStatus,
            component_status= componentStatus,
            visible=1,
            component_id=componentId };
            var data= JsonConvert.SerializeObject(obj);

            using(var wc=new WebClient())
            {
                wc.Headers.Add("X-Cachet-Token",tokenCachet);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var res= await wc.UploadStringTaskAsync(url,"POST" ,data);
            }
        }
        public async Task<int> CreateComponent(string name)
        {
            var url=urlCachet +"/api/v1/components";
            var parms= new NameValueCollection();
            parms.Add("name",name);
            parms.Add("status","1");
            using(var wc=new WebClient())
            {
                wc.Headers.Add("X-Cachet-Token",tokenCachet);
                var data= await wc.UploadValuesTaskAsync(url,"POST" ,parms);
                var str=Encoding.UTF8.GetString(data);
                var token=JToken.Parse(str);
                var id= token["data"]["id"].ToObject<int>();
                return id;
            }
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            DataTable dt;
            if(receiveData.Metadata.Tables.Count(it=>it.Name=="Cachet")==1)
                dt=receiveData.FindAfterName("Cachet").Value;
            else
                dt=receiveData.DataToBeSentFurther[0];

            var componentsList= await components();
            foreach(DataRow dr in dt.Rows)
            {
                var componentName=dr["component"].ToString();
                if(!componentsList.Exists(it=>it.Value == componentName))
                {
                    var id= await CreateComponent(componentName);
                    componentsList.Add(new KeyValuePair<long, string>(id,componentName));
                }
                var componentId= componentsList.First(it=>it.Value == componentName).Key;
                var incidentName = dr["incidentName"].ToString();
                var incidentMessage = dr["incidentMessage"].ToString();
                var incidentStatus= int.Parse(dr["incidentStatus"].ToString());
                var componentStatus =int.Parse(dr["componentStatus"].ToString());
                await CreateNewIncident(componentId,componentStatus, incidentName,incidentMessage,incidentStatus);



            }
            return receiveData;

        }

       

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
