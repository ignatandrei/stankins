
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
using StankinsObjects;

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
            var commands=new List<Type>();
            commands.Add (typeof(ReceiveMetadataFromDatabaseSql));
            commands.Add(typeof(SenderDBDiagramToDot));
            commands.Add(typeof(SenderDBDiagramHTMLDocument));
            commands.Add(typeof(ReceiveQueryFromFileSql));
            commands.Add(typeof(SenderAllTablesToFileCSV));
            commands.Add(typeof(ReceiveQueryFromFolderSql));
            commands.Add(typeof(SenderExcel));
            commands.Add(typeof(ExportDBDiagramHtmlAndDot));

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
                    commands.ForEach(it=>System.Console.WriteLine(it.Name));
                    
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
                        Type t = it;
                        System.Console.WriteLine(it.Name);
                        foreach (var ctor in t.GetConstructors())
                        {
                            if(ctor.IsPrivate)
                                continue;
                            if(ctor.IsStatic)
                                continue;

                            var paramsCtor = ctor.GetParameters();
                            if (paramsCtor.Length == 1)//eliminate default ctor with dataneeded dictionary
                            {
                                var param = paramsCtor[0];
                                if(param.Name.ToLower()=="dataneeded")
                                    continue;
                                
                            }
                            System.Console.WriteLine($"{ctor.MetadataToken})Arguments ");
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
                    IBaseObject last = null;
                    IDataToSent data = null;
                    var argNr = 0;
                    for (int i = 0; i < opt.Values.Count; i++)
                    {
                        
                        var item = opt.Values[i];
                        switch (item)
                        {
                            case nameof(ExportDBDiagramHtmlAndDot):
                            {
                                last = new ExportDBDiagramHtmlAndDot(argObjects.Values[argNr],
                                    argObjects.Values[argNr+1]);
                                argNr += 2;
                                data = await last.TransformData(data);
                            }
                                break;
                            case nameof(ReceiveMetadataFromDatabaseSql):
                            {
                                last = new ReceiveMetadataFromDatabaseSql(argObjects.Values[argNr]);
                                argNr++;
                                data = await last.TransformData(data);
                            }
                                break;
                            case nameof(SenderDBDiagramToDot):
                            {
                                last = new SenderDBDiagramToDot("");
                                data = await last.TransformData(data);
                               

                            }
                                break;
                            case nameof(SenderDBDiagramHTMLDocument):
                            {
                                last = new SenderDBDiagramHTMLDocument("");
                                data = await last.TransformData(data);
                            }
                                break;
                            case nameof(ReceiveQueryFromFileSql):
                            {
                                last= new ReceiveQueryFromFileSql(argObjects.Values[argNr],
                                    argObjects.Values[argNr + 1]);
                                argNr += 2;
                                data = await last.TransformData(data);
                            }
                                break;
                            case nameof(SenderAllTablesToFileCSV):
                            {
                                last = new SenderAllTablesToFileCSV(argObjects.Values[argNr]);
                                argNr++;
                                data = await last.TransformData(data);
                            }
                                break;
                            case nameof(ReceiveQueryFromFolderSql):
                            {
                                last = new ReceiveQueryFromFolderSql(argObjects.Values[argNr],
                                    argObjects.Values[argNr + 1], argObjects.Values[argNr + 2]);
                                argNr += 3;
                                data = await last.TransformData(data);
                                }
                                break;
                            case nameof(SenderExcel):
                            {
                                last = new SenderExcel(argObjects.Values[argNr]);
                                argNr++;
                                data = await last.TransformData(data);

                            }
                                break;
                            default:
                                System.Console.WriteLine($"not an existing object {item} -  see list");
                                break;
                        }
                    }

                   
                   var sender = last as ISender;
                   if (last == null)
                   {
                       System.Console.WriteLine("exporting default output");
                       sender=new SenderOutputToFolder("",true);
                       await sender.TransformData(data);
                   }
                    
                    return 0;
                });
            }
            );
            app.Execute(args);
        }
    }
}
