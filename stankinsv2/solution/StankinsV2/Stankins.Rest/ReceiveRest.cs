using Newtonsoft.Json.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Stankins.Rest
{
    public abstract class ReceiveRest : BaseObject, IReceive
    {
        public ReceiveRest(CtorDictionary dataNeeded) : base(dataNeeded)
        {
        }

        protected virtual IEnumerable<DataTable> FromJSon(string json)
        {
            JToken token = JToken.Parse(json);
            if (!(token is JArray))
            {
                json = "[" + json + "]";
                token = JToken.Parse(json);

            }
            JArray tok = token as JArray;
            return FromArray(tok);
            
        }
        private DataTable[] FromArray(JArray arr)
        {
            long id = 0;
            var tables = new Dictionary<string, DataTable>();
            JArray res = new JArray();
            bool HasArrayInside = false;
            foreach (JObject row in arr.Children())
            {
                HasArrayInside = false;
                JObject clean = new JObject();
                foreach (JProperty prop in row.Properties())
                {
                    if (prop.Value is JObject || prop.Value is JValue)
                    {
                        clean.Add(prop.Name, prop.Value);
                        continue;
                    }
                    if (prop.Value is JArray)
                    {
                        if (!HasArrayInside)
                        {
                            clean.Add("ID", ++id);
                            HasArrayInside = true;
                        }
                        DataTable[] dt = FromArray(prop.Value as JArray);
                        var count =dt.Count(it=>string.IsNullOrWhiteSpace(it.TableName));
                        if (count > 1)
                        {
                            string s=";";
                        }
                        var dtName = dt.First(it=>string.IsNullOrWhiteSpace(it.TableName));
                        dtName.TableName = prop.Name;
                        var dc=new DataColumn("Owner_ID",typeof(long));
                        dc.DefaultValue = id;
                        dtName.Columns.Add(dc);
                        foreach(var item in dt)
                        {
                            if (!tables.ContainsKey(item.TableName))
                            {
                                tables.Add(item.TableName,new DataTable(){ TableName = item.TableName });

                                
                            }
                            tables[item.TableName].Merge(item,true,MissingSchemaAction.Add);
                            if (tables.ContainsKey(""))
                            {
                                var rows=tables[""].Rows[0].ItemArray;
                                string s=string.Join(",",rows);
                            }
                        }
                        continue;
                    }
                    throw new ArgumentException("cannot understand json on " + prop.Name);
                }
                res.Add(clean);

            }
            DataTable ret = res.ToObject<DataTable>();
            if (tables.ContainsKey(""))
            {
                var rows=tables[""].Rows[0].ItemArray;
                string s=string.Join(",",rows);
            }
            tables.Add("",ret);
            return tables.Select(it=>it.Value).ToArray();
        }
        public abstract Task<string> GetData();

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            DataTable[] dt = FromJSon(await GetData()).ToArray();
            FastAddTables(receiveData, dt);
            return receiveData;

        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
