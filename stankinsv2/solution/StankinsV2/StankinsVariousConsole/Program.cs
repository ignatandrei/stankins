using SenderInterpretedRazor;
using Stankins.File;
using Stankins.HTML;
using Stankins.Interfaces;
using Stankins.Office;
using Stankins.Process;
using StankinsObjects;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsVariousConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        
        static async Task MainAsync(string[] args)
        {
            await BillGates();

            return;
            await Bookmarks();
            
            return;
            await ResultsDir();
            return;
            await BookerPrize();
            return;
            await Nobel();
            return;
            //var item = new DBReceiveTableNamesSqlServer("Server =.;Database=MyTestDatabase;Trusted_Connection=True;");
            //var data = await item.TransformData(null);

            //Console.WriteLine("1");

            //IReceive receive = new ReceiverCSV("OneTab.txt", Encoding.UTF8, false, '|', '\n');
            //var dataNoble = await receive.TransformData(null);

            //ITransformer separate = new SeparateByNumber(data.Metadata.Tables[0].Name, 10);
            //data = await separate.TransformData(data);
            //Console.WriteLine("2");
        }

        private static async Task BillGates()
        {
            var v = new Verifier();

            //var dt = new ReceiverHtmlAHref(@"https://www.gatesnotes.com/Books#All",Encoding.UTF8);
            //var dt = new ReceiverHtmlRegex(@"C:\Users\Surface1\Documents\bg.txt", Encoding.UTF8, @".(?:href=)(?<book>.+?)(?:#disqus).*?");
            var dt = new ReceiverHtmlRegex(@"C:\Users\Surface1\Documents\bg.txt", Encoding.UTF8, @".(?:href=\\"")(?<book>.+?)(?:#disqus).*?");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            var books = new RetainColumnDataContains(data.Metadata.Columns[0].Name, "ooks");
            data = await books.TransformData(data);
            await v.TransformData(data);
            var excel = new SenderExcel(@"bg.xslx");
            data = await excel.TransformData(data);
            data = await v.TransformData(data);
        }

        static async Task Bookmarks()
        {
            var v = new Verifier();

            var dt = new ReceiverHtmlSelector(@"C:\Users\Surface1\Desktop\bookmarks_11_17_17.html", Encoding.UTF8, "//dt/a");
            var data = await dt.TransformData(null);
            data = await v.TransformData(data);
            //data = await new TransformerHTMLAttribute("item_html", "href").TransformData(data);
            //data = await v.TransformData(data);
            data = await new TransformerAddColumnExpressionByColumn("item_html", "'<li>' + item_html +'</li>'", "li").TransformData(data);
            data = await v.TransformData(data);
            data = await new RemoveColumn("item_html").TransformData(data);
            data = await v.TransformData(data);
            data = await new RemoveColumn("item").TransformData(data);
            data = await v.TransformData(data);
            data = await new TransformerOneColumnToMultiTablesByNumber(data.Metadata.Tables.First().Name, 20).TransformData(data);
            data = await v.TransformData(data);

            var excel = new SenderExcel(@"text.xslx");
            data = await excel.TransformData(data);
            data = await v.TransformData(data);

        }

        static async Task ResultsDir()
        {
            var v = new Verifier();

            var r = new ReceiverProcess("print.exe",null);
            var data = await r.TransformData(null);
            await v.TransformData(data);
            return;
        }
        static async Task Nobel()
        {
            //var writeData = new SenderToConsole();
            var v = new Verifier();
            var nobelLiterature = new ReceiverHtmlTables("https://en.wikipedia.org/wiki/List_of_Nobel_laureates_in_Literature", Encoding.UTF8);
            var data = await nobelLiterature.TransformData(null);
            data = await v.TransformData(data);


            var f = new FilterTablesWithColumn("Laureate");
            data = await f.TransformData(data);
            data = await v.TransformData(data);


            var justSome = new FilterColumnData("Laureate", "Laureate not like '*ohn*'");
            data = await justSome.TransformData(data);
            data = await v.TransformData(data);

            var transform = new TransformerHTMLAttribute("Laureate_html", "href", "LaureateWiki");
            data = await transform.TransformData(data);
            data = await v.TransformData(data);

            var transformPicture = new TransformerHTMLAttribute("Picture_html", "src", "PictureUrl");
            data = await transformPicture.TransformData(data);
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
            //data = await (new TransformerAddColumnExpressionByColumn("li_html", "Len(li_html)", "liLen")).TransformData(data);
            //var csv = new SenderFileCSV(@"D:\test");
            //data = await csv.TransformData(data);
            data = await new RemoveColumn("li_html").TransformData(data);
            data = await new RemoveColumn("Picture").TransformData(data);
            data = await new RemoveColumn("Year_html").TransformData(data);
            data = await new RemoveColumn("Genre(s)_html").TransformData(data);
            data = await new RemoveColumn("LaureateWiki").TransformData(data);
            data = await new RemoveColumn("Country").TransformData(data);
            data = await new RemoveColumn("Picture_html").TransformData(data);
            data = await new RemoveColumn("Laureate_html").TransformData(data);
            data = await new RemoveColumn("Country_html").TransformData(data);
            data = await new RemoveColumn("Language(s)_html").TransformData(data);
            data = await new RemoveColumn("Citation_html").TransformData(data);

            data = await v.TransformData(data);
            //var regexLast = @"(?:.+\/)((?<name>.+))";
            data = await new AddColumnRegex("LaureateFullWiki_origin", @"(?:.+\/)((?<nameAuthor>.+))").TransformData(data);
            data = await v.TransformData(data);

            data = await new AddColumnRegex("LaureateFullWiki", @"(?:.+\/)((?<name>.+))").TransformData(data);

            data = await new RemoveColumn("LaureateFullWiki_origin").TransformData(data);

            data = await new ChangeTableNamesRegex(@"(?:.+\/)((?<name>.+))").TransformData(data);
            data = await v.TransformData(data);

            data = await new ChangeColumnName("li", "bookName").TransformData(data);
            data = await v.TransformData(data);

            data = await new SenderExcel(@"D:\test\nobel.xlsx").TransformData(data);


            data = await new FilterTablesWithColumn("bookName").TransformData(data);
            data = await v.TransformData(data);

            data = await new TransformerToOneTable().TransformData(data);
            data = await v.TransformData(data);

            var content = System.IO.File.ReadAllText("sqliteCreation.txt");
            data = await new SenderRazorTableOneByOne(content, @"D:\test\").TransformData(data);
            data = await v.TransformData(data);
            //data = await writeData.TransformData(data);




        }
        static async Task BookerPrize()
        {
            //var writeData = new SenderToConsole();
            var v = new Verifier();
            var booker = new ReceiverHtmlTables("https://en.wikipedia.org/wiki/Booker_Prize", Encoding.UTF8);
            var data = await booker.TransformData(null);

            data = await v.TransformData(data);

            data = await new FilterTablesWithColumn("Author").TransformData(data);
            data = await v.TransformData(data);

            data = await new FilterColumnData("Author", "Author not like '*ohn*'").TransformData(data);
            data = await v.TransformData(data);
            data = await new TransformerHTMLAttribute("Author_html", "href", "AuthorWiki").TransformData(data);
            data = await v.TransformData(data);
            data = await new TransformerAddColumnExpressionByTable(data.Metadata.Tables.First().Name, "'https://en.wikipedia.org'+ AuthorWiki ", "AuthorFullWiki").TransformData(data);
            data = await v.TransformData(data);
            var gatherLaureatesWiki = new TransformerOneTableToMulti<BaseObjectInSerial<ReceiverHtmlList, TransformerToOneTable>>(
                 "file", "AuthorFullWiki", new StankinsCommon.CtorDictionary()
                {
                    {nameof(Encoding),Encoding.UTF8 }
                }
                );
            data = await gatherLaureatesWiki.TransformData(data);
            data = await v.TransformData(data);
            data = await new FilterColumnDataWithRegex("li", @"(\([0-9]{4})|(, [0-9]{4})").TransformData(data);
            data = await v.TransformData(data);
            //data = await new ChangeTableNamesRegex(@"(?:.+\/)((?<name>.+))").TransformData(data);
            data = await v.TransformData(data);

            data = await new ChangeColumnName("li", "bookName").TransformData(data);
            data = await v.TransformData(data);
            data = await new RemoveColumn("li_html").TransformData(data);
            data = await new RemoveColumn("Year_html").TransformData(data);
            data = await new RemoveColumn("Author_html").TransformData(data);
            data = await new RemoveColumn("Title_html").TransformData(data);
            data = await new RemoveColumn("Genre(s)_html").TransformData(data);
            data = await new RemoveColumn("Country_html").TransformData(data);
            data = await v.TransformData(data);
            data = await (new TransformTrim()).TransformData(data);
            data = await v.TransformData(data);

            data = await new SenderExcel(@"D:\test\booker.xlsx").TransformData(data);
            data = await v.TransformData(data);
            var content = System.IO.File.ReadAllText("sqliteCreation.txt");
            data = await new SenderRazorTableOneByOne(content, @"D:\test\").TransformData(data);

        }
       
    }
}
