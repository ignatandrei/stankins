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

        protected virtual DataSet FromJSon(string json)
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
        private DataSet FromArray(JArray arr)
        {
            long id = 0;
            var tables = new DataSet();
            JArray res = new JArray();
            bool HasArrayInside = false;
            var relCols = new List<KeyValuePair<string,string>>();
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
                        var ds=FromArray(prop.Value as JArray);
                        var dt = ds.Tables.Cast<DataTable>().ToArray();
                        var dtName = dt.FirstOrDefault(it=>string.IsNullOrWhiteSpace(it.TableName) || it.TableName=="Table1");
                       
                        dtName.TableName = prop.Name;
                        var p=((arr.Parent as JProperty)?.Name ??"Owner");
                        var dc=new DataColumn(p+"ID",typeof(long));
                        dc.DefaultValue = id;
                        dtName.Columns.Add(dc);                        
                        tables.Merge(ds,true,MissingSchemaAction.Add);
                        
                        relCols.Add(new KeyValuePair<string, string>(prop.Name,dc.ColumnName));
                        continue;
                    }
                    throw new ArgumentException("cannot understand json on " + prop.Name);
                }
                res.Add(clean);

            }
            DataTable ret = res.ToObject<DataTable>();
            
            tables.Tables.Add(ret);
            foreach (var item in relCols.Distinct())
            {
                var colRel= tables.Tables[item.Key].Columns[item.Value];
                tables.Relations.Add("FK_ID_"+item.Key + "_"+item.Value ,ret.Columns["ID"],colRel);                
            }
            return tables;
        }
        public abstract Task<string> GetData();

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var ds = FromJSon(await GetData());
            
            var  dt=ds.Tables.Cast<DataTable>().ToArray();
            FastAddTables(receiveData, dt);
            var meta=receiveData.Metadata;
            foreach(DataRelation rel in ds.Relations)
            {
                meta.Relations.Add(new Relation()
                {
                    IdTableParent = receiveData.FindAfterName(rel.ParentTable.TableName).Key,
                    IdTableChild =receiveData.FindAfterName(rel.ChildTable.TableName).Key,
                    ColumnParent = rel.ParentColumns.First().ColumnName,
                    ColumnChild =rel.ChildColumns.First().ColumnName

                });
            }
            return receiveData;

        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
