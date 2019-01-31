
using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Stankins.Interfaces;
using Stankins.Razor;
using Stankins.SqlServer;
using System.IO;
using System.Linq;
using System.Reflection;
using Stankins.File;
using Stankins.Office;

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
            var commands=new List<string>();
            commands.Add (nameof(ReceiveMetadataFromDatabaseSql));
            commands.Add(nameof(SenderDBDiagramToDot));
            commands.Add(nameof(SenderDBDiagramHTMLDocument));
            commands.Add(nameof(ReceiveQueryFromFileSql));
            commands.Add(nameof(SenderAllTablesToFileCSV));
            commands.Add(nameof(ReceiveQueryFromFolderSql));
            commands.Add(nameof(SenderExcel));
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
                    commands.ForEach(System.Console.WriteLine);
                    
                    return 0;
                });

            });
            app.Command("explainAll", (command) =>
            {
                command.Description = "Explain arguments  for supported objects";
                command.HelpOption("-?|-h|--help");


                command.OnExecute(() =>
                {
                    commands.ForEach(it =>
                    {
                        Type t = Type.GetType(it);
                        System.Console.WriteLine(it);
                        foreach (var ctor in t.GetConstructors())
                        {
                            var paramsCtor = ctor.GetParameters();
                            if (paramsCtor.Length == 1)//eliminate default ctor with dataneeded dictionary
                            {
                                var param = paramsCtor[0];
                                if(param.Name.ToLower()=="dataneeded")
                                    continue;
                                
                            }

                            foreach (var parameterInfo in paramsCtor)
                            {
                                System.Console.WriteLine($"     {parameterInfo.Name}");
                            }
                        }

                       
                    });

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
                    IDataToSent data = null;
                    var argNr = 0;
                    for (int i = 0; i < opt.Values.Count; i++)
                    {
                        
                        var item = opt.Values[i];
                        switch (item)
                        {
                            case nameof(ReceiveMetadataFromDatabaseSql):
                            {
                                var baseObject = new ReceiveMetadataFromDatabaseSql(argObjects.Values[argNr]);
                                argNr++;
                                data = await baseObject.TransformData(data);
                            }
                                break;
                            case nameof(SenderDBDiagramToDot):
                            {
                                var baseObject = new SenderDBDiagramToDot("");
                                data = await baseObject.TransformData(data);
                                var f = baseObject.OutputContents.First();
                                System.IO.File.WriteAllText(f.Key + ".html", f.Value);

                            }
                                break;
                            case nameof(SenderDBDiagramHTMLDocument):
                            {
                                var baseObject = new SenderDBDiagramHTMLDocument("");
                                data = await baseObject.TransformData(data);
                                var f = baseObject.OutputContents.First();
                                System.IO.File.WriteAllText(f.Key + ".html", f.Value);
                            }
                                break;
                            case nameof(ReceiveQueryFromFileSql):
                            {
                                var baseObject = new ReceiveQueryFromFileSql(argObjects.Values[argNr],
                                    argObjects.Values[argNr + 1]);
                                argNr += 2;
                                data = await baseObject.TransformData(data);
                            }
                                break;
                            case nameof(SenderAllTablesToFileCSV):
                            {
                                var baseObject = new SenderAllTablesToFileCSV(argObjects.Values[argNr]);
                                argNr++;
                                data = await baseObject.TransformData(data);
                            }
                                break;
                            case nameof(ReceiveQueryFromFolderSql):
                            {
                                var baseObject = new ReceiveQueryFromFolderSql(argObjects.Values[argNr],
                                    argObjects.Values[argNr + 1], argObjects.Values[argNr + 2]);
                                argNr += 3;
                                data = await baseObject.TransformData(data);
                                }
                                break;
                            case nameof(SenderExcel):
                            {
                                var baseObject = new SenderExcel(argObjects.Values[argNr]);
                                argNr++;
                                data = await baseObject.TransformData(data);

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
