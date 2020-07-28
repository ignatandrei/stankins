using System;
using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class TransformerRenameLastTable : BaseObject, ITransformer
    {
        private readonly string newNameTable;

        public TransformerRenameLastTable(string newNameTable) : base(new CtorDictionary().AddMyValue(nameof(newNameTable), newNameTable))
        {

        }
        public TransformerRenameLastTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            newNameTable = GetMyDataOrThrow<string>(nameof(newNameTable));

        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var lastTable = receiveData.Metadata.Tables.Last();
            receiveData.DataToBeSentFurther[lastTable.Id].TableName = newNameTable;
            lastTable.Name = newNameTable;
            return Task.FromResult( receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}