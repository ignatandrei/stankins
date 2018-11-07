using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformSplitColumn : BaseObject, ITransformer
    {
        private readonly string nameTable;
        private readonly string nameColumn;
        private readonly char separator;

        public TransformSplitColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {
                this.nameTable = base.GetMyDataOrThrow<string>(nameof(nameTable));
                this.nameColumn = base.GetMyDataOrThrow<string>(nameof(nameColumn));
                this.separator = base.GetMyDataOrThrow<char>(nameof(separator));            
        }

        public TransformSplitColumn(string nameTable, string nameColumn, char separator)
            : this(new CtorDictionary()
        {

            { nameof(nameTable),nameTable },
            { nameof(nameColumn),nameColumn },
            { nameof(separator),separator }

        })

        {
            
        }



        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var table = receiveData.FindAfterName(nameTable);
            var id = table.Key;
            var tbl = table.Value;
            var cols = receiveData.Metadata.Columns.Where(it => it.IDTable == id).ToList();
            foreach (DataRow dr in tbl.Rows)
            {
                var val = dr[nameColumn]?.ToString();
                if (val.Length == 0)
                    continue;
                var arr = val.Split(separator);
                if (arr.Length == 1)
                    continue;
                for (int i = 0; i < arr.Length; i++)
                {
                    string newCol = nameColumn + "_" + i;
                    if (cols.Exists(it => it.Name == newCol))
                        continue;

                    tbl.Columns.Add(newCol, typeof(string));
                    var col = (new Column()
                    {
                        Name = newCol,
                        Id = cols.Count,
                        IDTable = id
                    });
                    cols.Add(col);
                    receiveData.Metadata.Columns.Add(col);

                }

            }
            foreach (DataRow dr in tbl.Rows)
            {
                var val = dr[nameColumn]?.ToString();
                if (val.Length == 0)
                    continue;
                var arr = val.Split(separator);
                if (arr.Length == 1)
                    continue;
                for (int i = 0; i < arr.Length; i++)
                {
                    string newCol = nameColumn + "_" + i;
                    dr[newCol] = arr[i];
                }
            }

            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
