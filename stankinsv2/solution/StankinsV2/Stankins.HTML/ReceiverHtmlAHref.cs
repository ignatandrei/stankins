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
    public class ReceiverHtmlAHref: Receiver
    {
        public ReceiverHtmlAHref(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
            File = GetMyDataOrThrow<string>(nameof(File));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);

        }
        public ReceiverHtmlAHref(string file, Encoding encoding) : this(new CtorDictionary()
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
            bool web = file.FileType == FileType.Web;
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            var links = doc.DocumentNode.SelectNodes("//a");

            if ((links?.Count ?? 0) == 0)
                throw new ArgumentException("not found a");

            var ret = new DataToSentTable();
            
            var dt = new DataTable
            {
                TableName = $"TableLinks"
            };
            
            dt.Columns.Add("href");
            dt.Columns.Add("a_html");
            dt.Columns.Add("a_text");
            
            var startWeb = "";
            if (web )
            {
                var uri = new Uri(File);
                startWeb = uri.GetLeftPart(UriPartial.Authority);
               }
            foreach (var link in links)
            {
                if (!link.Attributes.Contains("href"))
                {
                    //todo: put at error?
                    continue;
                }
               string val = link.Attributes["href"].Value;
               if (web && val.StartsWith("/"))
               {
                    val = (startWeb + val).Replace("//", "/");
                    
               }
                var item = new string[]
                {
                   val,
                   link.OuterHtml,
                   link.InnerText,

                };
                dt.Rows.Add(item);
               
               
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
