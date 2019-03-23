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
    public class TransformSplitColumnAddRow : BaseObject, ITransformer
    {
        private readonly string nameTable;
        private readonly string nameColumn;
        private readonly string separator;
        public TransformSplitColumnAddRow(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.nameTable = base.GetMyDataOrThrow<string>(nameof(nameTable));
            this.nameColumn = base.GetMyDataOrThrow<string>(nameof(nameColumn));
            this.separator = base.GetMyDataOrThrow<string>(nameof(separator));
            Name = nameof(TransformSplitColumn);
        }

        public TransformSplitColumnAddRow(string nameTable, string nameColumn, string separator)
            : this(new CtorDictionary()
                  .AddMyValue(nameof(nameTable),nameTable )
            .AddMyValue( nameof(nameColumn),nameColumn )
            .AddMyValue( nameof(separator),separator )

        )

        {
            
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var table = receiveData.FindAfterName(nameTable);
            var id = table.Key;
            var tbl = table.Value;
            var cols = receiveData.Metadata.Columns.Where(it => it.IDTable == id).ToList();
            var listNewValues = new List<object[]>();
            //finding position
            var pos = 0;
            for (var iCol = 0; iCol < tbl.Columns.Count; iCol++)
            {
                if (tbl.Columns[iCol].ColumnName == nameColumn)
                {
                    pos = iCol;
                    break;
                }
            }

            foreach (DataRow dr in tbl.Rows)
            {
                var val = dr[nameColumn]?.ToString();
                if ((val?.Length ?? 0) == 0)
                    continue;
                var arr = val.Split(new string[1]{ separator},StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 1)
                    continue;
                dr[nameColumn] = arr[0];
                // first item already put in previous line
                for (int i = 1; i < arr.Length; i++)
                {

                    var arrVal = dr.ItemArray;
                    arrVal[pos] = arr[i];
                    listNewValues.Add(arrVal);
                }

            }
            foreach(var newVal in listNewValues)
            {
                tbl.Rows.Add(newVal);
            }
            return await Task.FromResult( receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
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
            Name = nameof(TransformSplitColumn);
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
                if ((val?.Length ?? 0) == 0)
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
                if ((val?.Length ?? 0) == 0)
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

            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
