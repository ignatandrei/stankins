using System;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

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
            this.Name = nameof(FilterRenameTablesInOrder);
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
}