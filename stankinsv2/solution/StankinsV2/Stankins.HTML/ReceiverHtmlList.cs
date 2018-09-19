using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects ;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class ReceiverHtmlList : Receiver
    {
        public ReceiverHtmlList(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
            File = GetMyDataOrThrow<string>(nameof(File));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);

        }
        public ReceiverHtmlList(string file, Encoding encoding) : this(new CtorDictionary()
            {
                {nameof(file),file },
                {nameof(encoding),encoding },
            })
        {

        }

        public string File { get; }
        public Encoding Encoding { get; }
        public bool PrettifyColumnNames { get; set; } = true;
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        { 
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.File
            };

            var data = await file.LoadData();
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            var tables = doc.DocumentNode.SelectNodes("//ul | //ol | //dl");

            if ((tables?.Count ?? 0) == 0)
                throw new ArgumentException("not found ul, ol, dl");

            var ret = new DataToSentTable();

            int nrTable = 0;
            foreach (var table in tables)
            {
                nrTable++;
                var dt = new DataTable
                {
                    TableName = $"Table{nrTable}"
                };

                dt.Columns.Add("li");
                dt.Columns.Add("li_html");

                var rows = table.SelectNodes("li");
                if(rows?.Count >0)
                foreach (var row in rows)
                {                    
                    dt.Rows.Add(row.InnerText,row.InnerHtml);
                }
                var id= ret.AddNewTable(dt);
                ret.Metadata.AddTable(dt,id);
            }
            return ret;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

        private string MakePretty(string colName)
        {
            if (!PrettifyColumnNames)
                return colName;
            if (string.IsNullOrWhiteSpace(colName))
                return colName;
            colName = colName.Trim();
            colName = colName.Replace("\n", " ");
            while (colName.IndexOf("  ") > -1)
                colName = colName.Replace("  ", " ");

            return colName;
        }
    }
}
