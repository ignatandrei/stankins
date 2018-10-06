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
    public class TransformerHtmlAHref : BaseObject, ITransformer
    {
        public TransformerHtmlAHref(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(TransformerHtmlAHref);
            Content = GetMyDataOrThrow<string>(nameof(Content));
            

        }
        public TransformerHtmlAHref(string content) : this(new CtorDictionary()
            {
                {nameof(content),content},
                
            })
        {
            
        }

        public string Content { get; }

        public override async  Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
            {
                receiveData = new DataToSentTable();

            }
            var tr = new AHrefToTable();
            var dt = tr.TransformToTable(Content);

            var id = receiveData.AddNewTable(dt);
            receiveData.Metadata.AddTable(dt, id);
            return receiveData;
            
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
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
            var ret = new DataToSentTable();

            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.File
            };
            
            var data = await file.LoadData();
            bool web = file.FileType == FileType.Web;
           
            var startWeb = "";
            if (web)
            {
                var uri = new Uri(File);
                startWeb = uri.GetLeftPart(UriPartial.Authority);
            }
            var tr = new AHrefToTable();
            var dt = tr.TransformToTable(data);


            if (web) {
                foreach (DataRow item in dt.Rows)
                {

                    string val = item["href"]?.ToString();
                    if (val == null)
                        continue;

                    if (val.StartsWith("/"))
                    {
                        val = (startWeb + val).Replace("//", "/");

                    }
                    item["href"] = val;

                }
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
