
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
using Stankins.FileOps;
using Stankins.Office;
using StankinsCommon;
using StankinsObjects;
using StankinsHelperCommands;
using Stankins.AzureDevOps;
using System.Threading.Tasks;
using Stankins.Process;
using Stankins.Rest;
using Stankins.Version;
using Stankins.XML;
using Stankins.SimpleRecipes;
using Stankins.Interpreter;

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
            
            
            sb.AppendLine("SqlServer:");
            sb.AppendLine(
                $"     SqlServer1)To export diagram :{nl} stankins.console execute -o {nameof(ExportDBDiagramHtmlAndDot)} -a {connectionString} -a metadata.html");

            sb.AppendLine(
                $"     SqlServer2)To export a table to excel:{nl} stankins.console execute -o ExportTableToExcelSql -a {connectionString} -a nameofTheTable -a nameofTheTable.xlsx");
            sb.AppendLine(
                $"     SqlServer3)To export a table to csv:{nl} stankins.console execute -o ReceiveTableDatabaseSql -a {connectionString} -a nameofTheTable -o SenderAllTablesToFileCSV -a folderToSave");
            sb.AppendLine("Solution:");
            sb.AppendLine(
                $"     Solution1)stankins.console execute - o ReceiverFromSolution - a path.to.the.sln");

            return sb.ToString();

        }

        
        static void WriteLines(ResultTypeStankins[] b)
        {
            foreach (var it in b)
            {
                Type t = it.Type;
                System.Console.WriteLine("");
                System.Console.Write($"stankins.console execute -o {t.Name}");

                foreach (var item in it.ConstructorParam)
                {
                    System.Console.Write($" -a {item.Key}");
                }



            };
        }
        
        static void Main(string[] args)
        {

            var refs= FindAssembliesToExecute.AddReferences();

            var commands = new CtorDictionaryGeneric<ResultTypeStankins>();

            Action<ResultTypeStankins> createItem = (t) => { commands.Add(t.Name, t); };
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
            app.Command("recipes",(command)=>{
                command.Description = "execute/list recipes already in system" ;
                var list=command.Option("-l|--list","list recipes",CommandOptionType.NoValue);
                var execute=command.Option("-e|--execute","execute recipe",CommandOptionType.SingleValue);
                command.OnExecute(async () =>
                {
                    if(list.HasValue())
                    {
                        var recipes = RecipeFromString.RecipesFromFolder().ToList();
                        recipes.Sort((a,b)=>a.Name.CompareTo(b.Name));
                        foreach(var item in recipes)
                        {
                            System.Console.WriteLine($"Stankins.Console recipes -e {item.Name}");
                        }
                        return 0;
                    }
                    if (execute.HasValue())
                    {
                        var recipeString=  execute.Value();
                        var recipe=RecipeFromString.FindRecipe(recipeString);
                        if(recipe == null)
                        {
                            System.Console.Error.WriteLine($"can not found {recipeString}");
                            return 0;//maybe return error?
                        }   
                        await recipe.TransformData(null);
                        return 0;

                    }

                    command.ShowHelp();
                    return 0;
                });

            });
            app.Command("list", (command) =>
            {
                var names=string.Join(',', Enum.GetNames(typeof(WhatToList)));

                command.Description = "Explain arguments  for supported objects - could be " + names;

                command.HelpOption("-?|-h|--help"); 
                var optWhat = command.Option("-what", "what to list", CommandOptionType.SingleValue);
                


                command.OnExecute(() =>
                {
                    if (!optWhat.HasValue())
                    {
                        System.Console.WriteLine("please add -what " +names);
                        return -1;
                    }
                    var val =(WhatToList) (int) Enum.Parse(typeof(WhatToList), optWhat.Value(),true);
                    
                    var all = commands.Select(it => it.Value).ToList();

                    var find = all.Where(it => val == (val & it.FromType())).ToList();
                    find.Sort((a,b)=>a.Name.CompareTo(b.Name));

                    WriteLines(find.ToArray());


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

                            argNr += commands[item].ConstructorParam.Count;
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
                            if (cmd.ConstructorParam.Count > 0)
                            {
                                ctorArgs = new object[cmd.ConstructorParam.Count];
                                int i = 0;
                                do
                                {
                                    var item1= argObjects.Values[argNr];
                                    
                                    ctorArgs[i] = item1; 
                                    i++;
                                    argNr++;

                                } while (i < cmd.ConstructorParam.Count);
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
