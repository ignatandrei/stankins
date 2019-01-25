using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class ReceiverHtmlMeta : Receiver
    {
        public ReceiverHtmlMeta(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
            File = GetMyDataOrThrow<string>(nameof(File));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);

        }
        public ReceiverHtmlMeta(string file, Encoding encoding) : this(new CtorDictionary()
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
            var metas = doc.DocumentNode.SelectNodes("//head/meta");

            if ((metas?.Count ?? 0) == 0)
                throw new ArgumentException("not found head meta");

            var ret = new DataToSentTable();

            
            var dt = new DataTable
            {
                TableName = $"TableMeta"
            };
            dt.Columns.Add("meta_name");
            dt.Columns.Add("meta_content");
            if(metas != null)
            foreach (var meta in metas)
            {
                var attr = meta.Attributes;
                if (!attr.Contains("name"))
                    continue;
                var arr = new string[] { attr["name"].Value, attr["content"].Value };
                dt.Rows.Add(arr);
            }
            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            
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
