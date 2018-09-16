using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverFile
{
    public class ReceiverCSV : Receiver
    {
        public string FileCSV { get; }
        public Encoding Encoding { get; }
        public bool FirstLineHasColumnNames { get; }
        public char ColumnSeparator { get; }
        public char LineSeparator { get; }
        public ReceiverCSV(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSV);
            FileCSV = GetMyDataOrThrow<string>(nameof(FileCSV));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);
            ColumnSeparator = GetMyDataOrDefault<char>(nameof(ColumnSeparator), ',');
            LineSeparator = GetMyDataOrDefault<char>(nameof(LineSeparator), '\n');


        }
        public ReceiverCSV(string fileCSV, Encoding encoding, bool firstLineHasColumnNames, char columnSeparator, char lineSeparator) : this(new CtorDictionary()
            {
                {nameof(fileCSV),fileCSV },
                {nameof(encoding),encoding },
                {nameof(columnSeparator),columnSeparator },
                {nameof(lineSeparator),lineSeparator},
                {nameof(firstLineHasColumnNames),firstLineHasColumnNames }

            })
        {

        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var ret = new DataToSentTable();
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.FileCSV
            };
            var data = await file.LoadData();
            var splitLines = data.Split(this.LineSeparator);
            var dt = new DataTable
            {
                TableName = Path.GetFileNameWithoutExtension(FileCSV)
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
                            dt.Columns.Add(new DataColumn(item, typeof(string)));
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
                while(vals.Length > dt.Columns.Count)
                {
                    dt.Columns.Add($"Column{dt.Columns.Count+1}");
                }
                dt.Rows.Add(vals);
            }
            
            
            var id=ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            return ret;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            //TODO : read first line
            throw new NotImplementedException();
        }
    }
}