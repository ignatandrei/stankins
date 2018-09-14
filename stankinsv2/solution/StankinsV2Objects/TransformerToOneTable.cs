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
            var dt = receiveData.DataToBeSentFurther[0];
            for (int i = 1; i < receiveData.DataToBeSentFurther.Count; i++)
            {
                dt.Merge(receiveData.DataToBeSentFurther[i]);
            }
            while (receiveData.DataToBeSentFurther.Count != 1)
                receiveData.DataToBeSentFurther.Remove(receiveData.DataToBeSentFurther.Count - 1);
            //metadata
            for (int i = receiveData.Metadata.Tables.Count - 1; i >= 0; i--)
            {
                var t = receiveData.Metadata.Tables[i];
                if (t.Id == firstTableId)
                    continue;
                receiveData.Metadata.RemoveTable(t);
            }
                
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
