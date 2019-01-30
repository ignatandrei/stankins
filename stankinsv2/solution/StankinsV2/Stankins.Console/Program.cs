
using Microsoft.Extensions.CommandLineUtils;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using System.IO;
using System.Linq;
using System.Reflection;
using Stankins.File;

namespace Stankins.Console
{
    //https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
    class Program
    {
        

        static void Main(string[] args)
        {
            if (args?.Length < 1)
            {
                args =new[] { "-h" };
            }
            
            var app = new CommandLineApplication();
            app.Name = "Stankins.Console";
            var versionString = Assembly.GetEntryAssembly()
                                         .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                         .InformationalVersion
                                         .ToString();

            System.Console.WriteLine($"Stankins.Console v{versionString}");
            app.HelpOption("-?|-h|--help");

            app.Command("list", (command) =>
            {
                command.Description = "List all supported objects";
                command.HelpOption("-?|-h|--help");

                
                command.OnExecute(() =>
                {
                    System.Console.WriteLine(nameof(ReceiveMetadataFromDatabaseSql));
                    System.Console.WriteLine(nameof(SenderDBDiagramToDot));
                    System.Console.WriteLine(nameof(SenderDBDiagramHTMLDocument));
                    System.Console.WriteLine(nameof(ReceiveQueryFromFileSql));
                    System.Console.WriteLine(nameof(SenderAllTablesToFileCSV));
                    return 0;
                });

            });


            app.Command("execute", (command) =>
            {
                command.Description = "Execute multiple ";
                command.HelpOption("-?|-h|--help");
                var opt = command.Option("-o", "execute object", CommandOptionType.MultipleValue);
                var argObjects = command.Option("-a", "arguments for the object", CommandOptionType.MultipleValue);
                var locationArgument = command.Argument("[location]", "The object to execute -see list command ", true);

                command.OnExecute(async () =>
                {
                    if (!opt.HasValue())
                    {
                        System.Console.WriteLine("see list command for objects");
                        return 0;
                    }
                    IDataToSent data = null;
                    var argNr = 0;
                    for (int i = 0; i < opt.Values.Count; i++)
                    {
                        
                        var item = opt.Values[i];
                        switch (item)
                        {
                            case nameof(ReceiveMetadataFromDatabaseSql):
                                var r = new ReceiveMetadataFromDatabaseSql(argObjects.Values[argNr]);
                                argNr++;
                                data = await r.TransformData(data);
                                break;
                            case nameof(SenderDBDiagramToDot):
                            {
                                var dot = new SenderDBDiagramToDot("");
                                data = await dot.TransformData(data);
                                var f = dot.OutputContents.First();
                                System.IO.File.WriteAllText(f.Key + ".html", f.Value);

                            }
                                break;
                            case nameof(SenderDBDiagramHTMLDocument):
                            {
                                var ht = new SenderDBDiagramHTMLDocument("");
                                data = await ht.TransformData(data);
                                var f = ht.OutputContents.First();
                                System.IO.File.WriteAllText(f.Key + ".html", f.Value);
                            }
                                break;
                            case nameof(ReceiveQueryFromFileSql):
                            {
                                var ht = new ReceiveQueryFromFileSql(argObjects.Values[argNr],
                                    argObjects.Values[argNr + 1]);
                                argNr += 2;
                                data = await ht.TransformData(data);
                            }
                                break;
                            case nameof(SenderAllTablesToFileCSV):
                            {
                                var ht = new SenderAllTablesToFileCSV(argObjects.Values[argNr]);
                                argNr++;
                                data = await ht.TransformData(data);
                            }
                                break;
                            default:
                                System.Console.WriteLine($"not an existing object {item} -  see list");
                                break;
                        }
                    }
                    return 0;
                });
            }
            );
            app.Execute(args);
        }
    }
}
