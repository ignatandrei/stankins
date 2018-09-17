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
    //TODO:make one version that identifies what tables can be merged
    public class TransformerToOneTable : BaseObject, ITransformer
    {
        public TransformerToOneTable() : base(null)
        {
        }
        public TransformerToOneTable(CtorDictionary dataNeeded) : base(null)
        {
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData.DataToBeSentFurther.Count == 0)
                return receiveData;
            var firstTableId = receiveData.Metadata.Columns.Min(it => it.IDTable);
            var firstTableColumns = receiveData.Metadata
                .Columns.Where(it => it.IDTable == firstTableId)
                .Select(it => it.Name)
                .ToArray();
            //verify
            var first = receiveData.Metadata.Columns.FirstOrDefault(it => !firstTableColumns.Contains(it.Name));
            if(first != null)
            {
                throw new ArgumentException($"{string.Join(",", firstTableColumns)} does not contain { first.Name}");
            }
            var dt = receiveData.DataToBeSentFurther[firstTableId];
            foreach (var item in receiveData.DataToBeSentFurther)
            {
            
                if(item.Key != firstTableId)
                    dt.Merge(item.Value);

            }
            //remove all datatables but not firstTableId
            while (receiveData.DataToBeSentFurther.Count != 1)
            {
                var nr = receiveData.DataToBeSentFurther.First();
                if (nr.Key != firstTableId)
                    receiveData.DataToBeSentFurther.Remove(nr.Key);
                nr = receiveData.DataToBeSentFurther.Last();
                if (nr.Key != firstTableId)
                    receiveData.DataToBeSentFurther.Remove(nr.Key);
            }

            //metadata
            var cols = receiveData.Metadata.Columns;
            var tables = receiveData.Metadata.Tables;
            for (int i = tables.Count - 1; i >= 0; i--)
            {
                var t = tables[i];
                if (t.Id == firstTableId)
                    continue;

                receiveData.Metadata.RemoveTable(t);
                
                //var colsRemove = cols
                //    .Where(it => it.IDTable == t.Id)
                //    .Select(it=>it.Id)
                //    .ToArray();
                //for (int col = cols.Count - 1; col >= 0; col--)
                //{
                //    var colId = cols[col].Id;
                //    if (colsRemove.Contains(colId))
                //        cols.RemoveAt(i);
                //}
            }
                
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
