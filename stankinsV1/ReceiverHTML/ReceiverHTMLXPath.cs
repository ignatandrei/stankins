using HtmlAgilityPack;
using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverHTML
{
    public class ReceiverHTMLXPath : ReceiverFileFromStorage<FakeComparable>
    {
        public ReceiverHTMLXPath(string fileToRead, Encoding fileEnconding) :base(fileToRead,true, fileEnconding)
        {
            Name = $"load html with xpath";
            FileEnconding = Encoding.UTF8;
        }
        public string[] XPaths { get; set; }
        public string[] AttributeNames { get; set; }

        protected override async Task ProcessText(string text)
        {
            
            var ret = new List<IRowReceive>();
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var dn = doc.DocumentNode;
            var list = new List<HtmlNodeCollection>();
            foreach(var item in XPaths)
            {
                list.Add(dn.SelectNodes(item));
            }
            var nr = list.Min(it => it.Count);
            var nrCols = AttributeNames.Length;
            
            for (int i = 0; i < nr; i++)
            {
                var rr = new RowRead();
                for (int col = 0; col < nrCols; col++)
                {
                    string value;
                    //no attribute name means value
                    string attr = AttributeNames[col];
                    if (string.IsNullOrWhiteSpace(attr))
                    {
                        value = list[col][i].InnerText;
                        attr = "Value"+col;
                    }
                    else
                    {
                        value = list[col][i].Attributes[AttributeNames[col]].Value;
                    }
                    rr.Values.Add(attr, value);

                }
                ret.Add(rr);
            }
            valuesRead = ret.ToArray();
        }
    }
}
