using StankinsCommon;
using StankinsObjects;
using System.Text;

namespace Stankins.HTML
{
    public abstract class ReceiverHtml: Receiver
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
    }
}
