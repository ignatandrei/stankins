using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class ChangeColumnName: BaseObject, ITransformer
    {
        public string OldName { get; }
        public string NewName { get; }

        public ChangeColumnName(string oldName, string newName) : this(new CtorDictionary() {
            { nameof(oldName), oldName},
            { nameof(newName), newName}
            }
           )
        {

            
        }
        public ChangeColumnName(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            this.OldName = base.GetMyDataOrThrow<string>(nameof(OldName));
            this.NewName = base.GetMyDataOrThrow<string>(nameof(NewName));
            this.Name = nameof(ChangeColumnName);

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = base.FindTableAfterColumnName(OldName,receiveData);
            foreach (var item in tables)
            {
                var col = item.Value.Columns[OldName];
                col.ColumnName = NewName;
                
                var tbMeta = receiveData.Metadata.Columns.First(it => it.IDTable == item.Key && it.Name == OldName);
                tbMeta.Name = NewName;

            }

            return await Task.FromResult(receiveData);
        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}