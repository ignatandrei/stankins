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
    public class Verifier : BaseObject, ITransformer
    {
        public Verifier():base(null)
        {

        }
        public Verifier(CtorDictionary dataNeeded): base(null)
        {

        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var data = new DataToSentTable();
            var tables = receiveData.Metadata.Tables.Select(it => it.Name).ToList();
            if(tables.Count != receiveData.DataToBeSentFurther.Count)
            {
                throw new NotSupportedException($"Metadata has {tables.Count} tables, data has {receiveData.DataToBeSentFurther.Count} tables");
            }
            foreach(var item in receiveData.DataToBeSentFurther)
            {
                //verify name exists
                if (!tables.Contains(item.Value.TableName))
                {
                    throw new NotSupportedException($"{item.Value.TableName} is not found in metadata tables");
                }
                //verify tables
                var tablesFound = receiveData.Metadata.Tables.Where(t => t.Id == item.Key).ToArray();
                if(tablesFound.Length != 1)
                {
                    throw new NotSupportedException($"{item.Value.TableName} with id {item.Key} is retrieved {tablesFound.Length} times ( should be 1)");
                }
                
                //verify columns
                var cols = receiveData.Metadata
                    .Columns
                    .Where(it => it.IDTable == item.Key)
                    .Select(it=>it.Name)
                    .ToList();
                foreach(DataColumn dc in item.Value.Columns)
                {
                    if(!cols.Contains(dc.ColumnName))
                        throw new NotSupportedException($"{dc.ColumnName} is not into metadata columns ");
                    cols.Remove(dc.ColumnName);
                }
                if(cols.Count>0)
                    throw new NotSupportedException($"Data table does not contain {cols[0]} ");

             
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
