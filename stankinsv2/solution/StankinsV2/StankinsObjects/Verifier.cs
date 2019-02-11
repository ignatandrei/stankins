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
    public class Verifier : BaseObject, ITransformer
    {
        public Verifier():base(null)
        {

        }
        public Verifier(CtorDictionary dataNeeded): base(dataNeeded)
        {
            this.Name = nameof(Verifier);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var data = new DataToSentTable();
            var tablesMetadata = receiveData.Metadata.Tables.Select(it => it.Name).ToList();
            if(tablesMetadata.Count != receiveData.DataToBeSentFurther.Count)
            {
                throw new NotSupportedException($"Metadata has {tablesMetadata.Count} tables, data has {receiveData.DataToBeSentFurther.Count} tables");
            }
            var tablesData = receiveData.DataToBeSentFurther.Select(it => it.Value.TableName).ToArray();
            var except = tablesMetadata.Except(tablesData).ToArray();
            if (except.Length > 0)
                throw new NotSupportedException($"metadata tables has {except[0]} that is not a table in DataToBeSentFurther ");
            except = tablesData.Except(tablesMetadata).ToArray();
            if (except.Length > 0)
                throw new NotSupportedException($"table {except[0]} has no metadata");

            foreach (var item in receiveData.DataToBeSentFurther)
            {
                //verify name exists
                if (!tablesMetadata.Contains(item.Value.TableName))
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
                var colsMetadata = receiveData.Metadata
                    .Columns
                    .Where(it => it.IDTable == item.Key)
                    .Select(it=>it.Name)
                    .ToList();
                var colsTable = new List<string>();
                foreach(DataColumn dc in item.Value.Columns)
                {
                    colsTable.Add(dc.ColumnName);
                }
                if(colsTable.Count != colsMetadata.Count)
                    throw new NotSupportedException($"length not the same for cols {colsTable.Count} and metadata cols {colsMetadata.Count} from {item.Value.TableName} ");

                except = colsMetadata.Except(colsTable).ToArray();
                if(except.Length>0)
                    throw new NotSupportedException($"metadata has {except[0]} that is not a column in {item.Value.TableName} ");
                except = colsTable.Except(colsMetadata).ToArray();
                if (except.Length > 0)
                    throw new NotSupportedException($"cols for {item.Value.TableName} has {except[0]} that is not a metadata column ");
            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
