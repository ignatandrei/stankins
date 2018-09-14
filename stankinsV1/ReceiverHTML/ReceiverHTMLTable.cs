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
        public bool PrettifyColumnNames { get; set; } = true;
        protected override async Task ProcessText(string text)
        {
            var ret = new List<IRowReceive>();
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var tables = doc.DocumentNode.SelectNodes("//table");

            if ((tables?.Count ?? 0) == 0)
                throw new ArgumentException("not found tables");

            foreach(var table    in tables)
            {
                string[] columnsNames = null;
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
                            var colName = columns[i].InnerText;
                            columnsNames[i] =MakePretty(colName) ;
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
                            var colName = cells[i].InnerText;
                            columnsNames[i] =MakePretty( cells[i].InnerText);
                        }
                        continue;
                    }
                    //cells = row.SelectNodes("td");
                    if ((cells?.Count ?? 0) == 0)
                        continue;

                    var rr = new RowRead();
                    ret.Add(rr);
                    
                    for (int i = 0; i < cells.Count; i++)
                    {
                        var key = columnsNames[i];
                        if(string.IsNullOrWhiteSpace(key))
                        {
                            key = "!NoName";
                        }
                        if (rr.Values.ContainsKey(key))
                        {
                            rr.Values[key] += cells[i].InnerText;
                        }
                        else
                        {
                            rr.Values.Add(key, cells[i].InnerText);
                        }

                        
                    }
                    
                    
                }
               
            }
            valuesRead = ret.ToArray();
            await Task.CompletedTask;


        }

        private string MakePretty(string colName)
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
