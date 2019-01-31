using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformerOneTableToMulti<T> : BaseObject, ITransformer
        where T:IBaseObject
    {
        public  static readonly string separator="|";
        
        private readonly string ReceiverProperty;
        protected readonly string ColumnNameWithData;
        
        public TransformerOneTableToMulti(CtorDictionary data):base(data)
        {
            ReceiverProperty = GetMyDataOrThrow<string>(nameof(ReceiverProperty));
            ColumnNameWithData = GetMyDataOrThrow<string>(nameof(ColumnNameWithData));

        }
        public TransformerOneTableToMulti(string receiverProperty, string columnNameWithData , CtorDictionary dataNeeded):
            this(new CtorDictionary(dataNeeded)
            {
                { nameof(receiverProperty),receiverProperty },
                {nameof(columnNameWithData),columnNameWithData }
            })
        {
            
            
        }
        
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            string colName = ColumnNameWithData;
            string receiver = ReceiverProperty;
            //TODO : remove from release builds
            var v = new Verifier();
            if (ColumnNameWithData.Contains(separator))
            {
                var arr = ColumnNameWithData.Split(new []{separator},StringSplitOptions.RemoveEmptyEntries);
                colName = arr[0];
                base.dataNeeded[nameof(ColumnNameWithData)] = string.Join(separator, arr.Skip(1).ToArray());

            }

            if (ReceiverProperty.Contains(separator))
            {
                var arr = ReceiverProperty.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                receiver = arr[0];
                base.dataNeeded[nameof(ReceiverProperty)] = string.Join(separator, arr.Skip(1).ToArray());
            }
            var column = receiveData.Metadata.Columns.FirstOrDefault(it => it.Name == colName);
            if(column == null)
            {
                column = receiveData.Metadata.Columns.FirstOrDefault(it => it.Name.Equals(colName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (column == null)
            {
                throw new ArgumentException($"no column {colName} in metadata");

            }
            var table = receiveData.DataToBeSentFurther[column.IDTable];
            //TODO: verify null
            foreach (DataRow item in table.Rows)
            {
                object data=null;
                try
                {
                    data = item[column.Name];
                    var d = new CtorDictionary(dataNeeded)
                {
                    { receiver, data }
                };
                    var r = Activator.CreateInstance(typeof(T), d) as BaseObject;
                    r.Name = data.ToString();
                    //TODO : load this async all
                    var dataToBeSent = await r.TransformData(null);
                    //TODO : remove from release builds
                    await v.TransformData(dataToBeSent);
                    foreach (var dataTable in dataToBeSent.DataToBeSentFurther)
                    {
                        dataTable.Value.TableName += data.ToString();
                        var dc = new DataColumn(column.Name + "_origin", data.GetType())
                        {
                            DefaultValue = data
                        };
                        dataTable.Value.Columns.Add(dc);
                        var idTable = receiveData.AddNewTable(dataTable.Value);
                        var meta = dataToBeSent.Metadata.Tables.FirstOrDefault(it => it.Id == dataTable.Key);
                        meta.Name = dataTable.Value.TableName;
                        meta.Id = idTable;
                        receiveData.Metadata.Tables.Add(meta);
                        foreach (var c in dataToBeSent.Metadata.Columns)
                        {
                            c.IDTable = idTable;
                            receiveData.Metadata.Columns.Add(c);
                        }
                        receiveData.Metadata.Columns.Add(new Column()
                        {
                            Name = column.Name + "_origin",
                            Id = receiveData.Metadata.Columns.Count,
                            IDTable = idTable
                        });
                        //TODO: add relation
                    }
                    //TODO : remove from release builds                
                    await v.TransformData(receiveData);
                }
                catch (Exception ex)
                {
                    //TODO: log or put into error table
                    Console.WriteLine("error at " + column.Name+ "-" + data?.ToString() +"-" + ex.Message);
                    throw;
                }
            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
