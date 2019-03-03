using HtmlAgilityPack;
using StankinsCommon;
using StankinsObjects;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public abstract class ReceiverHtml: Receive
    {
        public ReceiverHtml(string file, Encoding encoding) : this(new CtorDictionary()
            {
                {nameof(file),file },
                {nameof(encoding),encoding },
            })
        {

        }
        public ReceiverHtml(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
            File = GetMyDataOrThrow<string>(nameof(File));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);

        }
        public string File { get; }
        public Encoding Encoding { get; }
        public bool PrettifyColumnNames { get; set; } = true;
        protected string MakePretty(string colName)
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
        protected async Task<HtmlNodeCollection> Find(string find)
        {
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.File
            };

            var data = await file.LoadData();
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            return  doc.DocumentNode.SelectNodes(find);
        }
    }
}
