using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class TransformerOneColumnToMultiTablesByNumber : BaseObject, ITransformer
    {
        public TransformerOneColumnToMultiTablesByNumber(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            NameTable = base.GetMyDataOrThrow<string>(nameof(NameTable));
            NrRows = base.GetMyDataOrThrow<int>(nameof(NrRows));
        }
        public TransformerOneColumnToMultiTablesByNumber(string nameTable, int nrRows) : this(new CtorDictionary()
        {
            {nameof(nameTable),nameTable},
            { nameof(nrRows),nrRows},
            
        })
        {
            
        }

        public string NameTable { get; }
        public int NrRows { get; }

        public async override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var table = receiveData.Metadata.Tables.FirstOrDefault(it=>it.Name == NameTable);
            if(table == null)
            {
                table = receiveData.Metadata.Tables.FirstOrDefault(it =>string.Equals( it.Name ,NameTable, StringComparison.CurrentCultureIgnoreCase));

            }
            if (table == null)
                throw new ArgumentException($"cannot find table {NameTable}");
            var nr = 0;
            var tableFound = receiveData.DataToBeSentFurther[table.Id];
            DataTable prevDataTable = null;
            foreach (DataRow dr in tableFound.Rows)
            {
                if(nr % NrRows == 0)
                {
                    if (prevDataTable != null) {
                        prevDataTable.TableName = $"{tableFound.TableName}_{nr-NrRows}_{nr-1}";
                        var id = receiveData.AddNewTable(prevDataTable);
                        receiveData.Metadata.AddTable(prevDataTable, id);
                    }
                    prevDataTable = tableFound.Clone();
                    
                }
                nr++;
                if (prevDataTable != null)
                    prevDataTable.Rows.Add(dr.ItemArray);
            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new System.NotImplementedException();
        }
    }
}
