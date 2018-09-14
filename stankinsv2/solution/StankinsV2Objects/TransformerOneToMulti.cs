using StankinsCommon;
using StankinsV2Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    public class TransformerOneTableToMulti<T> :  ITransformer
        where T:IBaseObject
    {
        
        private readonly string receiverProperty;
        protected readonly string columnNameWithData;
        protected readonly CtorDictionary dataNeeded;

        public TransformerOneTableToMulti(string receiverProperty, string columnNameWithData , CtorDictionary dataNeeded)
        {
            
            this.receiverProperty = receiverProperty;
            this.columnNameWithData = columnNameWithData;
            this.dataNeeded = dataNeeded;
        }
        public string Name { get ; set ; }
        public IDictionary<string,object> StoringDataBetweenCalls { get ; set ; }

        public Version Version { get; protected set; }

        public async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var column = receiveData.Metadata.Columns.FirstOrDefault(it => it.Name == columnNameWithData);
            if(column == null)
            {
                column = receiveData.Metadata.Columns.FirstOrDefault(it => it.Name.Equals(columnNameWithData, StringComparison.InvariantCultureIgnoreCase));
            }
            if (column == null)
            {
                throw new ArgumentException($"no column {columnNameWithData} in metadata");

            }
            var table = receiveData.DataToBeSentFurther[column.IDTable];
            //TODO: verify null
            foreach (DataRow item in table.Rows)
            {
                var data = item[column.Name];
                var d = new CtorDictionary(dataNeeded)
                {
                    { this.receiverProperty, data }
                };
                var r = Activator.CreateInstance(typeof(T), d) as BaseObject;
                r.Name = data.ToString();
                //TODO : load this async all
                var dataToBeSent= await r.TransformData(null);
                foreach(var dataTable in dataToBeSent.DataToBeSentFurther)
                {
                    dataTable.Value.TableName += data.ToString();
                    var dc= new DataColumn(column.Name + "_origin", data.GetType());
                    dc.DefaultValue = data;
                    dataTable.Value.Columns.Add(dc);
                    receiveData.DataToBeSentFurther.Add(receiveData.DataToBeSentFurther.Count, dataTable.Value);
                }
                //TODO: add relation
                foreach(var meta in dataToBeSent.Metadata.Tables)
                {
                    //remake the id
                    meta.Name += data.ToString();
                    dataToBeSent.Metadata.AssignNewId(meta, receiveData.Metadata.Tables.Count);
                    receiveData.Metadata.Tables.Add(meta);
                    foreach(var c in dataToBeSent.Metadata.Columns)
                    {
                        receiveData.Metadata.Columns.Add(c);
                    }
                    receiveData.Metadata.Columns.Add(new Column()
                    {
                        Name = column.Name+"_origin",
                        Id = receiveData.Metadata.Columns.Count,
                        IDTable = meta.Id
                    }
                     );
                    
                }
            }
            return receiveData;
        }

        public Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
