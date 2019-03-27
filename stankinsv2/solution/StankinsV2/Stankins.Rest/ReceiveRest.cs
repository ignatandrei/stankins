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
            DataSet tables = new DataSet();
            JArray res = new JArray();
            bool HasArrayInside = false;
            List<KeyValuePair<string, string>> relCols = new List<KeyValuePair<string, string>>();
            foreach (JObject row in arr.Children())
            {
                HasArrayInside = false;
                JObject clean = new JObject();
                foreach (JProperty prop in row.Properties())
                {
                    if (prop.Value is JObject || prop.Value is JValue)
                    {
                        clean.Add(prop.Name, prop.Value.ToString());
                        continue;
                    }
                    if (prop.Value is JArray)
                    {
                        if (!HasArrayInside)
                        {
                            clean.Add("ID", ++id);
                            HasArrayInside = true;
                        }
                        DataSet ds = FromArray(prop.Value as JArray);
                        DataTable[] dt = ds.Tables.Cast<DataTable>().ToArray();
                        if(dt.Length == 0)
                        {
                            continue;
                        }
                        var dtName = dt.FirstOrDefault(it => string.IsNullOrWhiteSpace(it.TableName) || it.TableName == "Table1");

                        dtName.TableName = prop.Name;
                        string p = ((arr.Parent as JProperty)?.Name ?? "Owner");
                        var dc = new DataColumn(p + "ID", typeof(long))
                        {
                            DefaultValue = id
                        };
                        dtName.Columns.Add(dc);
                        tables.Merge(ds, true, MissingSchemaAction.Add);

                        relCols.Add(new KeyValuePair<string, string>(prop.Name, dc.ColumnName));
                        continue;
                    }
                    throw new ArgumentException("cannot understand json on " + prop.Name);
                }
                res.Add(clean);

            }
            var ret = res.ToObject<DataTable>();
            if (ret.Columns.Count > 0)
            {
                tables.Tables.Add(ret);
                foreach (KeyValuePair<string, string> item in relCols.Distinct())
                {
                    DataColumn colRel = tables.Tables[item.Key].Columns[item.Value];
                    try
                    {
                        tables.Relations.Add("FK_ID_" + item.Key + "_" + item.Value, ret.Columns["ID"], colRel);
                    }
                    catch(Exception ex)
                    {
                        //TODO: find why is here an error
                        Console.WriteLine(ex.Message);
                    }
                }
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
            DataSet ds = FromJSon(await GetData());

            DataTable[] dt = ds.Tables.Cast<DataTable>().ToArray();
            FastAddTables(receiveData, dt);
            IMetadata meta = receiveData.Metadata;
            foreach (DataRelation rel in ds.Relations)
            {
                meta.Relations.Add(new Relation()
                {
                    IdTableParent = receiveData.FindAfterName(rel.ParentTable.TableName).Key,
                    IdTableChild = receiveData.FindAfterName(rel.ChildTable.TableName).Key,
                    ColumnParent = rel.ParentColumns.First().ColumnName,
                    ColumnChild = rel.ChildColumns.First().ColumnName

                });
            }
            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
