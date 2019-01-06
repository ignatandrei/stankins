using SenderInterpretedRazor;
using Stankins.Alive;
using Stankins.Amazon;
using Stankins.File;
using Stankins.HTML;
using Stankins.Interfaces;
using Stankins.Office;
using Stankins.Process;
using Stankins.WLW;
using Stankins.XML;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace StankinsVariousConsole
{
    abstract class Step
    {
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public string DisplayName { get; set; }
    }
    abstract class MultipleCommands: Step
    {

    }
    class Script : MultipleCommands
    {
        public Script(string value)
        {
            Name = "script";
            Value = value;
        }
    }
    class Bash : MultipleCommands
    {
        public Bash(string value)
        {
            Name = "bash";
            Value = value;
        }
    }
    class TaskYaml: Step
    {
        public TaskYaml(string value)
        {
            Name = "task";
            Value = value;
            Inputs = new List<KeyValuePair<string, string>>();
        }
        public List<KeyValuePair<string,string>> Inputs { get; set; }
    }
    class Powershell: MultipleCommands
    {
        public Powershell(string value)
        {
            Name = "powershell";
            Value = value;
        }
    }
    class Checkout: Step
    {
        public Checkout(string value)
        {
            Name = "checkout";
            Value = value;
        }
    }
    class JobYaml
    {
        public JobYaml()
        {
            DependsOn = new List<string>();
            Steps = new List<Step>();
        }
        public string condition;
        public List<string> DependsOn;
        public KeyValuePair<string, string> pool;
        public List<Step> Steps;
        public string Name { get; set; }
    }
    class YamlDevOpsVisitor : IYamlVisitor
    {
        public YamlDevOpsVisitor()
        {
            jobs = new List<JobYaml>();
        }
        public List<JobYaml> jobs;
        public void Visit(YamlStream stream)
        {
            Console.WriteLine("stream");
        }

        public void Visit(YamlDocument document)
        {
            Console.WriteLine("document");
        }

        public void Visit(YamlScalarNode scalar)
        {
            //Console.WriteLine("scalar" + scalar.Value);
        }

        public void Visit(YamlSequenceNode sequence)
        {
            //Console.WriteLine("sequence" + sequence.Style);
            foreach (var item in sequence.Children)
            {
                item.Accept(this);
            }
        }
        private JobYaml LastJob()
        {
            return this.jobs.LastOrDefault();
        }
        public void Visit(YamlMappingNode mapping)
        {
            
            foreach(var item in mapping.Children)
            {
                if(item.Key.NodeType == YamlNodeType.Scalar)
                {
                    var sc = item.Key as YamlScalarNode;
                    switch (sc.Value)
                    {
                        case "variables":
                            {
                                //TODO: variables
                                var job = LastJob();
                                if (job == null)
                                {
                                    //TODO: is the yaml name
                                    continue;
                                }
                                else
                                {
                                }
                            }
                            continue;
                        case "inputs":
                            {
                                var seq = item.Value as YamlMappingNode;
                                var t = LastJob().Steps.Last() as TaskYaml;
                                foreach (var inp in seq.Children)
                                {                             
                                    t.Inputs.Add(new KeyValuePair<string, string>( inp.Key.ToString(),inp.Value.ToString()));
                                }
                            }
                            continue;
                        case "displayName":
                        case "name":
                            {
                                var c = item.Value as YamlScalarNode;
                                var job = LastJob();
                                if (job == null)
                                {
                                    //TODO: is the yaml name
                                    continue;
                                }
                                else
                                {
                                    var step = job.Steps.LastOrDefault();
                                    if (step != null)
                                    {
                                        step.DisplayName = c.Value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("NOT FOUND STEP FOR " + c.Value);
                                    }
                                }
                            }
                            continue;
                        case "jobs":
                            item.Value.Accept(this);
                            continue;
                        case "job":
                            var j = new JobYaml();
                            var n = item.Value as YamlScalarNode;
                            j.Name = n.Value;
                            this.jobs.Add(j);                            
                            item.Value.Accept(this);
                            continue;
                        case "condition":
                            var s = item.Value as YamlScalarNode;
                            LastJob().condition = s.Value;
                            continue;
                        case "pool":
                            var name = item.Value as YamlMappingNode;
                            var vmimage = name.Children.First();
                            LastJob().pool = new KeyValuePair<string, string>(vmimage.Key.ToString(), vmimage.Value.ToString());
                            continue;
                        case "steps":
                            var steps = item.Value as YamlSequenceNode;
                            steps.Accept(this);
                            continue;
                        case "checkout":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Checkout(step.Value));
                            }
                            continue;
                        case "powershell":
                            { var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Powershell(step.Value));
                            }
                            continue;
                        case "bash":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Bash(step.Value));
                            }
                            continue;
                        case "script":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new Script(step.Value));
                            }
                            continue;
                        case "task":
                            {
                                var step = item.Value as YamlScalarNode;
                                LastJob().Steps.Add(new TaskYaml(step.Value));
                            }
                            continue;
                        case "dependsOn":
                            {
                                var l = LastJob();
                                var d = item.Value as YamlScalarNode;
                                if (d != null)
                                {
                                    l.DependsOn.Add(d.Value);
                                    continue;
                                }
                                var seq = item.Value as YamlSequenceNode;
                                foreach (var child in seq.Children)
                                {
                                    d = child as YamlScalarNode;
                                    l.DependsOn.Add(d.Value);
                                }
                            }
                            continue;
                        //foreach(var step in steps.Children)
                        //{
                        //    switch (step.NodeType)
                        //    {
                        //        case YamlNodeType.Mapping:

                        //            continue;

                        //        default:
                        //            Console.WriteLine("can not interpret step child " + step.NodeType);
                        //            continue;
                        //    }
                        //}
                        //continue;
                        default:
                            Console.WriteLine($" scalar not handled {sc.Value}");
                            continue;

                    }
                }
                Console.WriteLine("mapping");
                
            }
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            await Yaml();
            return;
            await OneTab();
            return;
            await WebSites();
            return;
            await PingSites();
            return;
            await jordanbpeterson();
            return;
            await Propriu();
            return;
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

        private static async Task Yaml()
        {
            var data = await File.ReadAllTextAsync("stankinsYaml.txt");
            var st = new StringReader(data);
            var yaml = new YamlStream();
            yaml.Load(st);
            var rt = yaml.Documents[0].RootNode;
            var visit = new YamlDevOpsVisitor();
            rt.Accept(visit);
            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(visit.jobs,Newtonsoft.Json.Formatting.Indented));
            //Console.WriteLine(rt);
            //foreach(var item in mp.Children)
            //{
            //    var q = item;
            //    InterpretNodeRoot(q);
            //}
        }
        //static void InterpretNodeRoot(KeyValuePair<YamlNode, YamlNode> rootNode)
        //{
        //    switch (rootNode.Key.NodeType)
        //    {
        //        case YamlNodeType.Scalar:
        //            var                  
        //    }
        //}
        private static async Task OneTab()
        {
            var v = new Verifier();
            //var dr = new ReceiverLinkOneTab("https://www.one-tab.com/page/4BuJyIbyQ7akwk0DrTLwUg");
            var dr = new ReceiverLinkOneTab("https://www.one-tab.com/page/2lpYRWu3R4CRTjAFCch5aA");
            var data = await dr.TransformData(null);
            await v.TransformData(data);

            data = await new RemoveColumn("href").TransformData(data);
            data = await new RemoveColumn("a_text").TransformData(data);
            await v.TransformData(data);
            var firstTableName = data.Metadata.Tables[0].Name;
            
            data =await new TransformerOneColumnToMultiTablesByNumber(firstTableName, 15).TransformData(data);
            await v.TransformData(data);

            data = await new FilterRemoveTable(firstTableName).TransformData(data);
            await v.TransformData(data);

            string file = Path.Combine(Directory.GetCurrentDirectory(), "onetab.xlsx");

            var excel = new SenderExcel(file);
            data = await excel.TransformData(data);
            await v.TransformData(data);

            data = await new FilterRenameTablesInOrder(304, "Friday Links ###").TransformData(data);
            await v.TransformData(data);

            data =await new SenderWindowsLiveWriter(null,"</li><li>","","<li>","").TransformData(data);
            await v.TransformData(data);


        }
        private static async Task WebSites()
        {
            var v = new Verifier();
            var dt = new ReceiverWeb("http://www.yahoo.com");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            string file = Path.Combine(Directory.GetCurrentDirectory(), "ping.xlsx");
            var excel = new SenderExcel(file);
            data = await excel.TransformData(data);


        }

        private static async Task PingSites()
        {
            var v = new Verifier();
            var dt = new ReceiverPing("www.yahoo.com");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            string file = Path.Combine(Directory.GetCurrentDirectory(), "ping.xlsx");
            var excel = new SenderExcel(file);
            data = await excel.TransformData(data);


        }
        private static async Task jordanbpeterson()
        {
            var v = new Verifier();
            var dt = new ReceiverHtmlList("https://jordanbpeterson.com/great-books/");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            data = await new TransformerToOneTable().TransformData(data);
            await v.TransformData(data);

            data = await new FilterRetainColumnDataContains("li_html", "http://amzn.to").TransformData(data);
            await v.TransformData(data);

            

            data = await new TransformSplitColumn(data.Metadata.Tables[0].Name, "li", ':').TransformData(data);
            string file = Path.Combine(Directory.GetCurrentDirectory(), "jordanbpeterson.xlsx");
            var excel = new SenderExcel(file);
            data = await excel.TransformData(data);
            data = await v.TransformData(data);


            
            Process.Start(@"C:\Program Files (x86)\Microsoft Office\root\Office16\excel.exe", file);
            



        }
        private static async Task Propriu()
        {
            var v = new Verifier();
            var dt = new ReceiverXML(@"C:\Users\Surface1\Downloads\blogpropriu.wordpress.2018-10-06.xml", Encoding.UTF8, @"//item/category[@nicename=""carti-5-stele""]/..");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            data = await new TransformerXMLToColumn("OuterXML", "//title", "title", ",").TransformData(data);
            await v.TransformData(data);
            data = await new TransformerXMLToColumn("OuterXML", "//category", "category", ",").TransformData(data);
            await v.TransformData(data);

            data = await new TransformerXMLToColumn("OuterXML", @"//*[name()=""content:encoded""]", "content", ",").TransformData(data);
            await v.TransformData(data);

            data =await  new TransformerOneTableToMulti<TransformerHtmlAHref>("Content", "content", new CtorDictionary()).TransformData(data);

            await v.TransformData(data);
            data = await new FilterTablesWithColumn("href").TransformData(data);

            await v.TransformData(data);
            data = await new TransformerToOneTable().TransformData(data);

            await v.TransformData(data);
            data = await new FilterRetainColumnDataContains("href","amazon").TransformData(data);
            //await v.TransformData(data);
            //data = await new RetainColumnDataContains("a_text", "Lord of Light").TransformData(data);
            await v.TransformData(data);
            data = await new TransformerOneTableToMulti<AmazonMeta>("file","href",new CtorDictionary()).TransformData(data);
            await v.TransformData(data);
            data = await new FilterTablesWithColumn("meta_content").TransformData(data);
            await v.TransformData(data);
            data = await new TransformerToOneTable().TransformData(data);
            await v.TransformData(data);
            var excel = new SenderExcel(@"andrei.xslx");
            data = await excel.TransformData(data);
            data = await v.TransformData(data);
            

        }

        private static async Task BillGates()
        {
            var v = new Verifier();

            //var dt = new ReceiverHtmlAHref(@"https://www.gatesnotes.com/Books#All",Encoding.UTF8);
            //var dt = new ReceiverHtmlRegex(@"C:\Users\Surface1\Documents\bg.txt", Encoding.UTF8, @".(?:href=)(?<book>.+?)(?:#disqus).*?");
            var dt = new ReceiverHtmlRegex(@"C:\Users\Surface1\Documents\bg.txt", Encoding.UTF8, @".(?:href=\\"")(?<book>.+?)(?:#disqus).*?");
            var data = await dt.TransformData(null);
            await v.TransformData(data);
            var books = new FilterRetainColumnDataContains(data.Metadata.Columns[0].Name, "ooks");
            data = await books.TransformData(data);
            await v.TransformData(data);
            var t = new TransformerOneTableToMulti<ReceiverHtmlMeta>("file", data.Metadata.Columns[0].Name,new CtorDictionary());
            data = await t.TransformData(data);
            await v.TransformData(data);
            data = await new FilterTablesWithColumn("meta_name").TransformData(data);
            await v.TransformData(data);
            data = await new FilterTablesWithColumn("meta_name").TransformData(data);
            await v.TransformData(data);
            data = await new TransformerToOneTable().TransformData(data);
            await v.TransformData(data);
            books = new FilterRetainColumnDataContains("meta_name", "keywords");            
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
