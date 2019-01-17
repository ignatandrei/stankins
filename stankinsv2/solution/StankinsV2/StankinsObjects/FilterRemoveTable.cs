using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class FilterRenameTablesInOrder : BaseObject, IFilter
    {
        public FilterRenameTablesInOrder(int nrStart, string formatName) : this(new CtorDictionary() {
            { nameof(formatName), formatName },
            {nameof(nrStart),nrStart }
            }
            )
        {

        }
        public FilterRenameTablesInOrder(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.FormatName = base.GetMyDataOrThrow<string>(nameof(FormatName));
            this.NrStart = base.GetMyDataOrThrow<int>(nameof(NrStart));
        }

        public string FormatName { get; }
        public int NrStart { get; }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var nrTables = receiveData.Metadata.Tables.Count;
            for (int i = 0; i < nrTables; i++)
            {
                var newName = (NrStart+i).ToString(FormatName); 
                
                var t = receiveData.Metadata.Tables[i];
                var t1 = receiveData.DataToBeSentFurther[t.Id];
                t.Name = newName;
                t1.TableName = newName;

            }

            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }

    public class FilterRemoveTable : BaseObject, IFilter
    {
        public FilterRemoveTable(string nameTable) : this(new CtorDictionary() {
            { nameof(nameTable), nameTable }
            }
            )
        {

        }
        public FilterRemoveTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameTable = base.GetMyDataOrThrow<string>(nameof(NameTable));
        }

        public string NameTable { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var id = receiveData.FindAfterName(NameTable);
            receiveData.DataToBeSentFurther.Remove(id.Key);
            
            receiveData.Metadata.Tables.RemoveAt(id.Key);
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
