using Stankins.HTML;
using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Amazon
{
    public class AmazonMeta: ReceiverHtmlMeta
    {
        public AmazonMeta(CtorDictionary dataNeeded) : base(dataNeeded)
        {
           

        }
        public AmazonMeta(string file, Encoding encoding) : this(new CtorDictionary()
            {
                {nameof(file),file },
                {nameof(encoding),encoding },
            })
        {

        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var data=  await base.TransformData(receiveData);
            var table = data.FindAfterName("TableMeta");
            string title = "";
            foreach(DataRow dr in table.Rows)
            {
                if (dr["meta_name"]?.ToString() != "title")
                    continue;

                title = dr["meta_content"]?.ToString();
                break;

            }
            var items = title.Split(':')
                .Where(it=>!string.IsNullOrWhiteSpace(it))
                .Select(it=>it?.Trim())
                .ToArray();
            table.Rows.Add(new[] { "category", items[items.Length-1] });
            table.Rows.Add(new[] { "author", items[2] });
            table.Rows.Add(new[] { items[items.Length - 1], items[1] });//Books 
            return data;

        }
    }
}
