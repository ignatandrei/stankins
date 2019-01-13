using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformerToOneTableHierarchical : BaseObject, ITransformer
    {
        private readonly string nameTable1;
        private readonly string field1;
        private readonly string nameTable2;
        private readonly string field2;
        private readonly string nameTableResult;

        public TransformerToOneTableHierarchical(CtorDictionary dataNeeded):base(dataNeeded)
        {
            this.nameTable1 = GetMyDataOrThrow<string>(nameof(nameTable1));
            this.field1 = GetMyDataOrThrow<string>(nameof(field1));
            this.nameTable2 = GetMyDataOrThrow<string>(nameof(nameTable2));
            this.field2 = GetMyDataOrThrow<string>(nameof(field2));
            this.nameTableResult = GetMyDataOrThrow<string>(nameof(nameTableResult));

        }
        public TransformerToOneTableHierarchical(string nameTable1, string field1, string nameTable2,string field2, string nameTableResult)
            :this( new CtorDictionary()
            {
                {nameof(nameTable1),nameTable1 },
                {nameof(field1),field1},

                {nameof(nameTable2),nameTable2 },
                {nameof(field2),field2},
                {nameof(nameTableResult),nameTableResult},


            })
        {
            
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var table1 = receiveData.FindAfterName(nameTable1);
            var table2 = receiveData.FindAfterName(nameTable2);
            var ds = new DataSet();
            ds.Tables.Add(table1.Value.Copy());
            ds.Tables.Add(table2.Value.Copy());
            DataColumn dc1 =null, dc2=null;
            foreach(DataColumn dc in ds.Tables[0].Columns)
            {
                if(dc.ColumnName == field1)
                {
                    dc1 = dc;
                    break;
                }
            }
            foreach (DataColumn dc in ds.Tables[1].Columns)
            {
                if (dc.ColumnName == field2)
                {
                    dc2 = dc;
                    break;
                }
            }
            //var dret = new DataRelation("rel", table1.Value.TableName, table2.Value.TableName, new[] { field1 }, new[] { field2 },false);
            var dret = new DataRelation("rel", dc1, dc2);
            ds.Relations.Add(dret);

            var dt = new DataTable(nameTableResult);
            foreach(DataColumn col in table1.Value.Columns)
            {
                dt.Columns.Add($"{nameTable1}_{col.ColumnName}", col.DataType);
            }

            foreach (DataColumn col in table2.Value.Columns)
            {
                dt.Columns.Add($"{nameTable2}_{col.ColumnName}", col.DataType);
            }

            foreach(DataRow drParent in ds.Tables[0].Rows)
            {
                var rowsParent= drParent.ItemArray;
                foreach(var drChild in drParent.GetChildRows(dret))
                {
                    var rowsChild = drChild.ItemArray;
                    var obj= new object[rowsParent.Length + rowsChild.Length];
                    rowsParent.CopyTo(obj, 0);
                    rowsChild.CopyTo(obj, rowsParent.Length);
                    dt.Rows.Add(obj);
                }
            }
            var id = receiveData.AddNewTable(dt);
            receiveData.Metadata.AddTable(dt, id);
            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
