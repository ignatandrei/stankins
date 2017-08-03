using HtmlAgilityPack;
using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReceiverBookmarkExportChrome
{
    public class ReceiverBookmarkFileChrome : ReceiverFileFromStorage<DateTime>
    {

        public ReceiverBookmarkFileChrome(string fileName):base(fileName,readAllFirstTime: true,fileEnconding:Encoding.UTF8)
        {
        
        }
        

        protected override async Task ProcessText(string text)
        {
            List<IRowReceive> returns = new List<IRowReceive>();
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            
            var hrefs=doc.DocumentNode.SelectNodes("//a");
            foreach(var item in hrefs)
            {
                var rr = new RowRead();
                rr.Values.Add("href", item.Attributes["HREF"].Value);
                rr.Values.Add("title", item.InnerText);
                rr.Values.Add("ADD_DATE", item.Attributes["ADD_DATE"].Value);
                returns.Add(rr);
            }
            valuesRead = returns.ToArray();
        }
    }
}
