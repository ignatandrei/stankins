
using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Cont;
using NPOI.OpenXmlFormats.Wordprocessing;
using Stankins.AnalyzeSolution;
using Stankins.File;
using Stankins.Office;
using StankinsCommon;
using StankinsObjects;
using StankinsHelperCommands;
using Stankins.AzureDevOps;
using System.Threading.Tasks;
using Stankins.Process;
using Stankins.Version;
using Stankins.XML;

namespace Stankins.Console
{
    //https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
    class Program
    {

        static string ExtendedHelpText()
        {
            string connectionString = "\"Server=..;Database=..;User Id=...;Password=...\"";
            //docker example
            //connectionString = "\"Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;\"";
            string nl = Environment.NewLine;
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("Some fast usage  ");
            
            
            sb.AppendLine("1)SqlServer:");
            sb.AppendLine(
                $"     1.1)To export diagram :{nl} stankins.console execute -o {nameof(ExportDBDiagramHtmlAndDot)} -a {connectionString} -a metadata.html");

            sb.AppendLine(
                $"     1.2)To export a table to excel:{nl} stankins.console execute -o ExportTableToExcelSql -a {connectionString} -a nameofTheTable -a nameofTheTable.xlsx");
            sb.AppendLine(
                $"     1.3)To export a table to csv:{nl} stankins.console execute -o ReceiveTableDatabaseSql -a {connectionString} -a nameofTheTable -o SenderAllTablesToFileCSV -a folderToSave");
            sb.AppendLine("2)Solution:");
            sb.AppendLine(
                $"     2.2)execute - o ReceiverFromSolution - a path.to.the.sln");

            return sb.ToString();

        }

        private class CreatorBaseObject
        {
            public readonly Type typeOfObject;
            public readonly CtorDictionary arguments;

            public CreatorBaseObject(Type typeOfObject, CtorDictionary arguments)
            {
                this.typeOfObject = typeOfObject;
                this.arguments = arguments;
            }


            public BaseObject Create(object[] ctorStrings)
            {
                if (ctorStrings?.Length != arguments.Count())
                {
                    throw new ArgumentException($"number of args {arguments} != {ctorStrings?.Length}");
                }

                BaseObject act;
                if (ctorStrings?.Length > 0)
                {
                    act = Activator.CreateInstance(typeOfObject, ctorStrings) as BaseObject;

                }
                else
                {
                    act = Activator.CreateInstance(typeOfObject) as BaseObject;

                }

                return act;
            }

        }
        static void WriteLines(CreatorBaseObject[] b)
        {
            foreach (var it in b)
            {
                Type t = it.typeOfObject;
                System.Console.WriteLine("");
                System.Console.Write($"- o {t.Name}");

                foreach (var item in it.arguments)
                {
                    System.Console.Write($" -a {item.Key}");
                }



            };
        }
        static KeyValuePair<Type, CtorDictionary>[] AddReferences()
        {
            var allTypes = new List<KeyValuePair<Type, CtorDictionary>>();
           

            FindAssembliesToExecute f;

            f = new FindAssembliesToExecute(typeof(Stankins.Amazon.AmazonMeta).Assembly);

            allTypes.AddRange( f.FindAssemblies());

            f = new FindAssembliesToExecute(typeof(Stankins.AnalyzeSolution.ReceiverFromSolution).Assembly);

            allTypes.AddRange( f.FindAssemblies());

            f = new FindAssembliesToExecute(typeof(Stankins.AzureDevOps.YamlReader).Assembly);

            allTypes.AddRange( f.FindAssemblies());

          

            f = new FindAssembliesToExecute(typeof(Stankins.File.ReceiverCSV).Assembly);

            allTypes.AddRange( f.FindAssemblies());

            f = new FindAssembliesToExecute(typeof(Stankins.HTML.ReceiverHtml).Assembly);

            allTypes.AddRange( f.FindAssemblies());
            
            f = new FindAssembliesToExecute(typeof(Stankins.Office.SenderExcel).Assembly);

            allTypes.AddRange( f.FindAssemblies());
           

            f = new FindAssembliesToExecute(typeof(Stankins.Process.ReceiverProcess).Assembly);

            allTypes.AddRange( f.FindAssemblies());

            f = new FindAssembliesToExecute(typeof(Stankins.Razor.SenderDBDiagramToDot).Assembly);

            allTypes.AddRange( f.FindAssemblies());

            f = new FindAssembliesToExecute(typeof(Stankins.SqlServer.ExportDBDiagramHtmlAndDot).Assembly);

            allTypes.AddRange( f.FindAssemblies());
           
            f = new FindAssembliesToExecute(typeof(Stankins.Version.FileVersionFromDir).Assembly);

            allTypes.AddRange( f.FindAssemblies());
           
            f = new FindAssembliesToExecute(typeof(Stankins.XML.ReceiverXML).Assembly);

            allTypes.AddRange( f.FindAssemblies());




            return allTypes.ToArray() ;
        }
        static void Main(string[] args)
        {

            var refs= AddReferences();

            var commands = new CtorDictionaryGeneric<CreatorBaseObject>();

            Action<KeyValuePair<Type, CtorDictionary>> createItem = (t) => { commands.Add(t.Key.Name, new CreatorBaseObject(t.Key, t.Value)); };
            foreach (var item in refs)
            {
                createItem(item);
            }
          

            var app = new CommandLineApplication();
            app.Name = "Stankins.Console";
            var versionString = Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion
                .ToString();

           

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", app.Name + "v" + versionString, app.Name + "v" + versionString);
            app.ExtendedHelpText = ExtendedHelpText();
            //app.Command("list", (command) =>
            //{
            //    command.Description = "List all supported objects";
            //    command.HelpOption("-?|-h|--help");


            //    command.OnExecute(() =>
            //    {
            //        var all = commands.Select(it => it.Key.ToString()).ToList();
            //        all.Sort();
            //        all.ForEach(it => System.Console.WriteLine(it));

            //        return 0;
            //    });

            //});
            app.Command("list", (command) =>
            {
                command.Description = "Explain arguments  for supported objects";
                command.HelpOption("-?|-h|--help"); 


                command.OnExecute(() =>
                {
                    
                    var all = commands.Select(it => it.Value).ToList();
                    var receivers = all.Where(it => typeof(IReceive).IsAssignableFrom(it.typeOfObject)).ToArray();
                    System.Console.WriteLine("");
                    System.Console.WriteLine("!start with receivers");

                    System.Console.WriteLine("");
                    WriteLines(receivers);

                    System.Console.WriteLine("");
                    System.Console.WriteLine("!then with transformers, filters");
                    System.Console.WriteLine("");

                  

                    var next = all.Where(it => 
                        typeof(IFilter).IsAssignableFrom(it.typeOfObject)
                        ||
                        typeof(ITransformer).IsAssignableFrom(it.typeOfObject)
                    ).ToArray();
                    WriteLines(next);

                    var sender = all.Where(it =>
                        typeof(ISender).IsAssignableFrom(it.typeOfObject)
                    ).ToArray();

                    System.Console.WriteLine("");
                    System.Console.WriteLine("!then with senders");
                    System.Console.WriteLine("");
                    WriteLines(sender);


                    System.Console.WriteLine("");
                    System.Console.WriteLine("!for often used  commands , see -h option");
                    return 0;
                });

            });


            app.Command("execute", (command) =>
                {
                    command.Description = "Execute multiple ";
                    command.HelpOption("-?|-h|--help");
                    var opt = command.Option("-o", "execute object", CommandOptionType.MultipleValue);
                    var argObjects = command.Option("-a", "arguments for the object", CommandOptionType.MultipleValue);
                    //var locationArgument = command.Argument("[location]", "The object to execute -see list command ", true);

                    command.OnExecute(async () =>
                    {
                        if (!opt.HasValue())
                        {
                            System.Console.WriteLine("see list command for objects");
                            return 0;
                        }


                        var argNr = 0;
                        var lenValuesCount = opt.Values.Count;
                        for (int i = 0; i < lenValuesCount; i++)
                        {

                            var item = opt.Values[i].ToLowerInvariant();
                            if (!commands.ContainsKey(item))
                            {
                                System.Console.WriteLine($"not an existing object {item} - please see list command");
                                return -1;
                            }

                            argNr += commands[item].arguments.Count;
                        }

                        if (argNr != argObjects.Values.Count)
                        {
                            System.Console.WriteLine($"not equal nr args -a {lenValuesCount} with  nr args for objects {argNr} - please see list command");
                            return -1;
                        }
                    

                        IBaseObject last = null;
                        IDataToSent data = null;
                        argNr = 0;
                        for (int value = 0; value < lenValuesCount; value++)
                        {

                            var item = opt.Values[value].ToLowerInvariant();
                            var cmd = commands[item];
                            object[] ctorArgs = null;
                            if (cmd.arguments.Count > 0)
                            {
                                ctorArgs = new object[cmd.arguments.Count];
                                int i = 0;
                                do
                                {
                                    var item1= argObjects.Values[argNr];
                                    
                                    ctorArgs[i] = item1; 
                                    i++;
                                    argNr++;

                                } while (i < cmd.arguments.Count);
                            }
                            
                            last = cmd.Create(ctorArgs);
                            data = await last.TransformData(data);


                        }

                       
                        var output = last as ISenderToOutput;
                        if (output == null)
                        {
                            System.Console.WriteLine("exporting default output");
                            var sender = new SenderOutputToFolder("", true);
                            data=await sender.TransformData(data);
                            //System.Console.WriteLine("exporting all tables to csv");
                            //await new SenderAllTablesToFileCSV("").TransformData(data);

                        }

                        return 0;
                    });
                }
            );

            if (args?.Length < 1)
            {
                app.ShowRootCommandFullNameAndVersion();
                app.ShowHint();

                return;
            }

            app.Execute(args);
        }
    }
}
