using HtmlAgilityPack;
using ReceiverFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverFile
{
    public class ReceiverHTMLTable : ReceiverFileFromStorage<FakeComparable>
    {
        public ReceiverHTMLTable(string fileToRead, Encoding fileEnconding) : base(fileToRead,true, fileEnconding)
        {
            
        }

        protected override async Task ProcessText(string text)
        {
            var ret = new List<IRowReceive>();
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var tables = doc.DocumentNode.SelectNodes("//table");
            string[] columnsNames = null;
            foreach(var table    in tables)
            {
                var head = table.SelectSingleNode("thead");
                if (head == null)
                {
                    //TODO: log head not found
                    continue;
                }
                var body = table.SelectSingleNode("tbody");
                if (body == null)
                {
                    //TODO: log body not found
                    continue;
                }
                var columns = head.SelectNodes("tr/td");
                if(columns == null)
                {
                    //TODO: log header columns not found
                    continue;
                }
                columnsNames = new string[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsNames[i] = columns[i].InnerText;
                }
                var rows = body.SelectNodes("tr");
                foreach (var row in rows)
                {
                    var rr = new RowRead();
                    ret.Add(rr);
                    var cells = row.SelectNodes("td");
                    for (int i = 0; i < cells.Count; i++)
                    {
                        rr.Values.Add(columnsNames[i], cells[i].InnerText);
                    }
                    
                }
                valuesRead = ret.ToArray();
                await Task.CompletedTask;
            }


        }
    }
}
