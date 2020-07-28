using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class TransformerRenameTable : BaseObject, ITransformer
    {
        private readonly string newNameTable;
        private readonly string funcNameTableToBool;

        public TransformerRenameTable(
            string funcNameTableToBool,
            string newNameTable
            
            ) : this(new CtorDictionary()
                .AddMyValue(nameof(newNameTable), newNameTable)
                .AddMyValue(nameof(funcNameTableToBool), funcNameTableToBool)
                )
        {

        }
        public TransformerRenameTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            newNameTable = GetMyDataOrThrow<string>(nameof(newNameTable));
            funcNameTableToBool = GetMyDataOrThrow<string>(nameof(funcNameTableToBool));
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var criteria = await CSharpScript.EvaluateAsync<Func<string, bool>>(funcNameTableToBool);

            var tableFound = receiveData.Metadata.Tables.First(it => criteria(it.Name));
            receiveData.DataToBeSentFurther[tableFound.Id].TableName = newNameTable;

            tableFound.Name = newNameTable;
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
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