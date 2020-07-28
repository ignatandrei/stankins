using System;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class TransformerRenameTablesInOrder : BaseObject, ITransformer
    {
        public TransformerRenameTablesInOrder(int nrStart, string formatName) : this(new CtorDictionary() {
                { nameof(formatName), formatName },
                {nameof(nrStart),nrStart }
            }
        )
        {

        }
        public TransformerRenameTablesInOrder(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FormatName = base.GetMyDataOrThrow<string>(nameof(FormatName));
            NrStart = base.GetMyDataOrThrow<int>(nameof(NrStart));
            Name = nameof(TransformerRenameTablesInOrder);
        }

        public string FormatName { get; }
        public int NrStart { get; }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            int nrTables = receiveData.Metadata.Tables.Count;
            for (int i = 0; i < nrTables; i++)
            {
                string newName = (NrStart + i).ToString(FormatName);

                ITable t = receiveData.Metadata.Tables[i];
                System.Data.DataTable t1 = receiveData.DataToBeSentFurther[t.Id];
                t.Name = newName;
                t1.TableName = newName;

            }

            return await Task.FromResult(receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}