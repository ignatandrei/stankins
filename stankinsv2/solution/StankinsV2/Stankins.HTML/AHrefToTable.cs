using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Stankins.HTML
{
    //TODO: this is called from 2 parts. How to make one?
    class AHrefToTable
    {
        public DataTable TransformToTable(string data)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            var links = doc.DocumentNode.SelectNodes("//a");

            //if ((links?.Count ?? 0) == 0)
            //    throw new ArgumentException("not found a");
            var dt = new DataTable
            {
                TableName = $"TableLinks"
            };

            dt.Columns.Add("href");
            dt.Columns.Add("a_html");
            dt.Columns.Add("a_text");
            if ((links?.Count ?? 0) > 0)
                foreach (var link in links)
            {
                if (!link.Attributes.Contains("href"))
                {
                    //todo: put at error?
                    continue;
                }
                string val = link.Attributes["href"].Value;                
                var item = new string[]
                {
                   val,
                   link.OuterHtml,
                   link.InnerText,

                };
                dt.Rows.Add(item);


            }
            return dt;
        }
    }
}
