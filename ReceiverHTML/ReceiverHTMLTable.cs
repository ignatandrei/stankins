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
                if (head != null)
                {
                    var columns = head.SelectNodes("tr/th");
                    if((columns?.Count??0) == 0)
                    {
                        columns = head.SelectNodes("tr/td");
                    }
                    if (columns != null)
                    {

                        columnsNames = new string[columns.Count];
                        for (int i = 0; i < columns.Count; i++)
                        {
                            columnsNames[i] = columns[i].InnerText;
                        }
                    }
                }
                HtmlNodeCollection rows;
                var body = table.SelectSingleNode("tbody");
                if (body != null)
                {
                    rows = body.SelectNodes("tr");
                }
                else
                {
                    rows = table.SelectNodes("tr");
                }
                
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    if((cells?.Count??0) == 0)
                    {
                        cells = row.SelectNodes("th");
                    }
                    if ((columnsNames?.Length??0) == 0)
                    {
                        columnsNames = new string[cells.Count];
                        for (int i = 0; i < columnsNames.Length; i++)
                        {
                            columnsNames[i] = cells[i].InnerText;
                        }
                        continue;
                    }
                    cells = row.SelectNodes("td");
                    var rr = new RowRead();
                    ret.Add(rr);
                    
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
