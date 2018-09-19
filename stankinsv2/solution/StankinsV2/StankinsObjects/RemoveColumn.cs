using Stankins.Interfaces;
using StankinsCommon;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    //TODO: make remove more columns at once
    public class RemoveColumn: BaseObject, IFilter
    {
        public string NameColumn { get; }
        public RemoveColumn(string nameColumn) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn }
            }
              )
        {

        }
        public RemoveColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
        }

        public async override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = base.FindTableAfterColumnName(NameColumn, receiveData).ToArray();
            foreach (var tb in tables)
            {
                tb.Value.Columns.Remove(NameColumn);
            }
            //metadata
            var cols = receiveData.Metadata.Columns;
            for (int i = cols.Count - 1; i >= 0; i--)
            {
                if (!string.Equals(cols[i].Name, NameColumn))
                    continue;
                cols.RemoveAt(i);

            }
            return receiveData;


        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
