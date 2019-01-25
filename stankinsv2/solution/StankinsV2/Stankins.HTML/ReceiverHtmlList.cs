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
    public class ReceiverHtmlList : ReceiverHtml
    {
        public ReceiverHtmlList(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverHtmlTables);

        }
        public ReceiverHtmlList(string file) : this(file, Encoding.UTF8)
        {

        }
        public ReceiverHtmlList(string file, Encoding encoding) : base(file,encoding)
        {

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = await base.Find("//ul | //ol | //dl");

            if ((tables?.Count ?? 0) == 0)
                throw new ArgumentException("not found ul, ol, dl");

            var ret = new DataToSentTable();

            int nrTable = 0;
            foreach (var table in tables)
            {
                nrTable++;
                var dt = new DataTable
                {
                    TableName = $"Table{nrTable}"
                };

                dt.Columns.Add("li");
                dt.Columns.Add("li_html");

                var rows = table.SelectNodes("li");
                if (rows?.Count > 0)
                {
                    foreach (var row in rows)
                    {
                        dt.Rows.Add(row.InnerText, row.InnerHtml);
                    }
                }
                var id= ret.AddNewTable(dt);
                ret.Metadata.AddTable(dt,id);
            }
            return ret;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

        
    }
}
