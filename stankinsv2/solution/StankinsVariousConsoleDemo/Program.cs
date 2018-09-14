using DBReceiveSqlServer;
using ReceiverFile;
using ReceiverHTML;
using SenderConsole;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransformerGrouping;

namespace StankinsVariousConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            var writeData = new SenderToConsole();
            var v = new Verifier();
            var nobelLiterature = new ReceiverHtmlTables("https://en.wikipedia.org/wiki/List_of_Nobel_laureates_in_Literature", Encoding.UTF8);
            var data = await nobelLiterature.TransformData(null);
            data = await v.TransformData(data);
           
            
            var f = new FilterTablesWithColumn("Laureate");
            data= await f.TransformData(data);
            data = await v.TransformData(data);
            

                var justSome = new FilterColumnData("Laureate", "Laureate not like '*ohn*'");
            data = await justSome.TransformData(data);
            data = await v.TransformData(data);

            var transform = new TransformerHTMLAttribute("Laureate_html", "href","LaureateWiki");
            data =await transform.TransformData(data);
            data = await v.TransformData(data);

            var addSite = new TransformerAddColumnExpressionByTable(data.Metadata.Tables.First().Name, "'https://en.wikipedia.org'+ LaureateWiki ", "LaureateFullWiki");
            data = await addSite.TransformData(data);
            data = await v.TransformData(data);

            var gatherLaureatesWiki = new TransformerOneTableToMulti<BaseObjectInSerial<ReceiverHtmlList, TransformerToOneTable>>(
                "file", "LaureateFullWiki", new StankinsCommon.CtorDictionary()
                {
                    {nameof(Encoding),Encoding.UTF8 }
                }
                );
            data = await gatherLaureatesWiki.TransformData(data);
            data = await v.TransformData(data);
            //var h = new ReceiverHtmlList("https://en.wikipedia.org/wiki/Sully_Prudhomme",Encoding.UTF8);
            //var data2 = await h.TransformData(null);
            var yearFilter = new FilterColumnDataWithRegex("li_html", @"(\([0-9]{4})|(, [0-9]{4})");
            data = await yearFilter.TransformData(data);
            data = await v.TransformData(data);
            var italicFilter = new FilterColumnDataWithRegex("li_html", @"[<]i[>]");
            data = await italicFilter.TransformData(data);
            data = await v.TransformData(data);
            data = await (new TransformTrim()).TransformData(data);
            data = await v.TransformData(data);
            data = await (new FilterColumnDataGreaterThanLength("li_html", 400)).TransformData(data);
            data = await (new TransformerAddColumnExpressionByColumn("li_html", "Len(li_html)", "liLen")).TransformData(data);
            var csv = new SenderFileCSV(@"D:\test");
            data = await csv.TransformData(data);
            data = await writeData.TransformData(data);
            return;
            var item = new DBReceiveTableNamesSqlServer("Server=.;Database=MyTestDatabase;Trusted_Connection=True;");
            var data1 = await item.TransformData(null);

            Console.WriteLine("1");

            IReceive receive = new ReceiverCSV("OneTab.txt",Encoding.UTF8,false,'|', '\n');
            var dataNoble= await receive.TransformData(null);

            ITransformer separate = new SeparateByNumber(data.Metadata.Tables[0].Name, 10);
            data = await separate.TransformData(data);
            Console.WriteLine("2");
        }
    }
}
