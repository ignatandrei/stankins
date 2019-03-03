using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Stankins.XML
{
    public class ReceiverXML : Receive
    {
        public ReceiverXML(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverXML);
            File = GetMyDataOrThrow<string>(nameof(File));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);
            XPath = GetMyDataOrThrow<string>(nameof(XPath));
        }
        public ReceiverXML(string file, string xPath) : this(new CtorDictionary()
        {
            {nameof(file),file },
            {nameof(xPath),xPath},
        })
        {

        }
        public ReceiverXML(string file, Encoding encoding, string xPath) : this(new CtorDictionary()
            {
                {nameof(file),file },
                {nameof(encoding),encoding },
                {nameof(xPath),xPath},
            })
        {

        }
        public string File { get; }
        public Encoding Encoding { get; }
        public string XPath { get; set; }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            //var file = new ReadFileToString
            //{
            //    FileEnconding = this.Encoding,
            //    FileToRead = this.File
            //};

            //var data = await file.LoadData();
            var ret = new DataToSentTable();
            var dt = new DataTable
            {
                TableName = $"XML"
            };
            
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");
            dt.Columns.Add("OuterXML");
            XPathDocument document;
            try
            {
                document = new XPathDocument(File);
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"cannot load {File} because {ex.Message}",ex);
            }
            var navig = document.CreateNavigator();
            var nodes = navig.Select(XPath);
          
            while (nodes.MoveNext())
            {
                var n = nodes.Current;
                dt.Rows.Add(new[] { n.Name,n.Value, n.OuterXml });
            }
            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            return await Task.FromResult(ret);
        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}