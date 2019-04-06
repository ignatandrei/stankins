using Stankins.Interfaces;
using StankinsCommon;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class FilterRemoveColumns : BaseObject, IFilter
    {
        public string NameColumns { get; }
        public string Separator { get; set; }
        public FilterRemoveColumns(string nameColumns, string separator) : this(new CtorDictionary() {
            { nameof(nameColumns), nameColumns },
            {nameof(separator),separator }
            }
        )
        {

        }
        public FilterRemoveColumns(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameColumns = base.GetMyDataOrThrow<string>(nameof(NameColumns));
            this.Separator = base.GetMyDataOrThrow<string>(nameof(Separator));
            this.Name = nameof(FilterRemoveColumns);
        }
        public async override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var cols = NameColumns.Split(new string[] { Separator },StringSplitOptions.RemoveEmptyEntries);
            foreach(var item in cols)
            {
                receiveData = await new FilterRemoveColumn(item).TransformData(receiveData);
            }
            return receiveData;
        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

    }
    
    public class FilterRemoveColumn : BaseObject, IFilter
    {
        public string NameColumn { get; }
        public FilterRemoveColumn(string nameColumn) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn }
            }
              )
        {

        }
        public FilterRemoveColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
            this.Name = nameof(FilterRemoveColumn);
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
            return await Task.FromResult(receiveData);


        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
