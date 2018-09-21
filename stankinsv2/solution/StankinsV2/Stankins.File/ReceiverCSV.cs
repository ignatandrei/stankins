using StankinsCommon;
using Stankins.Interfaces;
using StankinsObjects ;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.File
{
    public class ReceiverStreamingText : IStreamingReceive<string>
    {
        public ReceiverStreamingText(string text, char separator)
        {
            Text = text;
            Separator = separator;
        }

        public string Text { get; }
        public Encoding Encoding { get; }
        public char Separator { get; }

        public async Task<bool> Initialize()
        {
            return true;
        }


        public async Task<string[]> StreamData()
        {
            var splitLines = Text.Split(Separator);
            return splitLines;
        }
    }
        public class ReceiverCSV : IStreamingReceive<string>
    {
        public ReceiverCSV(string nameFile, Encoding encoding,char separator)
        {
            NameFile = nameFile;
            Encoding = encoding;
            Separator = separator;
        }

        public string NameFile { get;  }
        public Encoding Encoding { get; }
        public char Separator { get; }

        public async Task<bool> Initialize()
        {
            return true;
        }


        public async Task<string[]> StreamData()
        {
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.NameFile
            };
            var data = await file.LoadData();
            var splitLines = data.Split(Separator);
            return splitLines;
        }
    }
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
            var ret = new DataToSentTable();
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
    public class ReceiverCSVText : ReceiveCSV<ReceiverStreamingText>
    {
        public ReceiverCSVText(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSVText);
            this.Text = GetMyDataOrThrow<string>(nameof(Text));

        }
        public ReceiverCSVText(string text,  bool firstLineHasColumnNames, char columnSeparator, char lineSeparator) : this(new CtorDictionary()
            {
                {nameof(text),text},
                {nameof(columnSeparator),columnSeparator },
                {nameof(lineSeparator),lineSeparator},
                {nameof(firstLineHasColumnNames),firstLineHasColumnNames }

            })
        {
            
        }

        public string Text { get; }

        public override ReceiverStreamingText Create()
        {
            return new ReceiverStreamingText(Text, LineSeparator);
        }

        public override string NameTable()
        {
            return "Table1";
        }
    }
    public class ReceiverCSVFile : ReceiveCSV<ReceiverCSV>
    {
        public string FileCSV { get; }
        public Encoding Encoding { get; }
        public ReceiverCSVFile (CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSVFile);
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);
            FileCSV = GetMyDataOrThrow<string>(nameof(FileCSV));
            

        }
        public ReceiverCSVFile(string fileCSV, Encoding encoding, bool firstLineHasColumnNames, char columnSeparator, char lineSeparator) : this(new CtorDictionary()
            {
                {nameof(fileCSV),fileCSV },
                {nameof(encoding),encoding },
                {nameof(columnSeparator),columnSeparator },
                {nameof(lineSeparator),lineSeparator},
                {nameof(firstLineHasColumnNames),firstLineHasColumnNames }

            })
        {

        }
        

        public override Task<IMetadata> TryLoadMetadata()
        {
            //TODO : read first line
            throw new NotImplementedException();
        }

        public override ReceiverCSV Create()
        {
            var s = new ReceiverCSV(this.FileCSV, this.Encoding, this.LineSeparator);
            return s;
        }

        public override string NameTable()
        {
            return Path.GetFileNameWithoutExtension(this.FileCSV);
        }
    }
}