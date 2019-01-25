using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class ReceiverHtmlSelector: ReceiverHtml
    {
        public ReceiverHtmlSelector(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);
            XPathExpression = GetMyDataOrThrow<string>(nameof(XPathExpression));
        }
        public ReceiverHtmlSelector(string file, Encoding encoding,string xPathExpression) : this(new CtorDictionary()
            {
                {nameof(file),file },
                {nameof(encoding),encoding },
            {nameof(xPathExpression),xPathExpression }
            })
        {
            
        }

        public string XPathExpression { get; }

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
            var nodes = doc.DocumentNode.SelectNodes(XPathExpression);
            //maybe return empty table?
            if ((nodes?.Count ?? 0) == 0)
                throw new ArgumentException($"not found {XPathExpression}");

            var ret = new DataToSentTable();

            int nrTable = 0;

            var dt = new DataTable
            {
                TableName = $"Table{nrTable}"
            };
            dt.Columns.Add("item");
            dt.Columns.Add("item_html");
            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            if (nodes != null)
            {
                foreach (var node in nodes)
                {

                    dt.Rows.Add(new[] { node.InnerText, node.OuterHtml });
                }
            }
            return ret;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            //todo: add item and item_html
            throw new NotImplementedException();
        }

       
    }
}
