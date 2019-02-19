using StankinsCommon;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Stankins.FileOps
{
    public abstract class ReceiveCSV<T>: Receiver
        where T:IStreamingReceive<string>
    {
        public char ColumnSeparator { get; protected set; }
        public bool FirstLineHasColumnNames { get; protected set; }
        public char LineSeparator { get; protected set; }

        public ReceiveCSV(CtorDictionary dataNeeded):base(dataNeeded)
        {
            ColumnSeparator = GetMyDataOrDefault<char>(nameof(ColumnSeparator), ',');
            LineSeparator = GetMyDataOrDefault<char>(nameof(LineSeparator), '\n');
            FirstLineHasColumnNames = GetMyDataOrDefault<bool>(nameof(FirstLineHasColumnNames), true);

        }
        public abstract T Create();
        public abstract string NameTable();
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var ret = receiveData?? new DataToSentTable();
            var file = Create();
            var splitLines = await file.StreamData();
            var dt = new DataTable
            {
                TableName = NameTable()
            };
            for (var i = 0; i < splitLines.Length; i++)
            {
                var vals = splitLines[i].Split(this.ColumnSeparator);
                if (i == 0)
                {
                    if (FirstLineHasColumnNames)
                    {
                        foreach (var item in vals)
                        {
                            dt.Columns.Add(new DataColumn(item.Trim(), typeof(string)));
                        }
                    }
                    else
                    {
                        for (int col = 0; col < vals.Length; col++)
                        {
                            dt.Columns.Add(new DataColumn($"Column{col + 1}", typeof(string)));
                        }
                        dt.Rows.Add(vals);
                    }

                    continue;
                }
                //case when length is not the same first time
                while (vals.Length > dt.Columns.Count)
                {
                    dt.Columns.Add($"Column{dt.Columns.Count + 1}");
                }
                dt.Rows.Add(vals);
            }


            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            return ret;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}