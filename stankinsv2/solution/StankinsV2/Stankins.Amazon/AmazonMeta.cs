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
            var data = await base.TransformData(receiveData);
            var table = data.FindAfterName("TableMeta").Value;
            string title = "";
            foreach (DataRow dr in table.Rows)
            {
                if (dr["meta_name"]?.ToString() != "title")
                    continue;

                title = dr["meta_content"]?.ToString();
                break;

            }
            var items = title.Split(':')
                .Where(it => !string.IsNullOrWhiteSpace(it))
                .Select(it => it?.Trim())
                .ToArray();
            //TODO:
            /*
             Plato and a Platypus Walk into a Bar: Understanding Philosophy Through Jokes - Kindle edition by Thomas Cathcart, Daniel Klein. Literature &amp; Fiction Kindle eBooks @ Amazon.com.
Next - Kindle edition by Michael Crichton. Mystery, Thriller &amp; Suspense Kindle eBooks @ Amazon.com.
Discover Your Genius: How to Think Like History&#39;s Ten Most Revolutionary Minds - Kindle edition by Michael J. Gelb. Religion &amp; Spirituality Kindle eBooks @ Amazon.com.
Irrationally Yours: On Missing Socks, Pickup Lines, and Other Existential Puzzles, Dan Ariely, William Haefeli - Amazon.com

*/
            switch (items.Length) {
                case 4:
                    //TODO: Le Livre de Volupte: Ahmad Ibn Souleiman: Amazon.com: Books
                    table.Rows.Add(new[] { "category", items[items.Length - 1] });
                    table.Rows.Add(new[] { "author", items[2] });
                    table.Rows.Add(new[] { items[items.Length - 1], items[1] });//Books 
                    break;
                case 5:

                    if (items[0].Contains("Amazon.com"))
                    {
                        table.Rows.Add(new[] { items[items.Length - 1], items[1]+items[2] });//Books 
                        table.Rows.Add(new[] { "author", items[3] });
                        table.Rows.Add(new[] { "category", items[items.Length - 1] });
                    }
                    else
                    {
                        table.Rows.Add(new[] { "category", items[items.Length - 1] });
                        table.Rows.Add(new[] { "author", items[1] });
                        table.Rows.Add(new[] { items[items.Length - 1], items[0] });//Books 
                    }
                    break;
                case 6:
                    {
                        var item = items[items.Length - 3]?.Trim();
                        if (!long.TryParse(item, out var _))
                            throw new ArgumentException("Do not know how to parse " + title);

                        table.Rows.Add(new[] { "category", items[items.Length - 1] });
                        table.Rows.Add(new[] { "author", items[items.Length - 4] });
                        table.Rows.Add(new[] { items[items.Length - 1], items[0] + items[1] });
                    }
                    break;
                case 7:
                    {
                        var item = items[items.Length - 3]?.Trim();
                        if (!long.TryParse(item, out var _))
                            throw new ArgumentException("Do not know how to parse " + title);

                        table.Rows.Add(new[] { "category", items[items.Length - 1] });
                        table.Rows.Add(new[] { "author", items[items.Length - 4] });
                        table.Rows.Add(new[] { items[items.Length - 1], items[0] + items[1] + item[2] });
                    }
                    break;

                case 2://Irrationally Yours: On Missing Socks, Pickup Lines, and Other Existential Puzzles, Dan Ariely, William Haefeli - Amazon.com
                    throw new ArgumentException("Just one : " + title);
                default:
                    throw new ArgumentException("Cannot interpret amazon title " + title);
            }
            return data;

        }
    }
}
