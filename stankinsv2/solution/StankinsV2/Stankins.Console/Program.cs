
using Microsoft.Extensions.CommandLineUtils;
using Stankins.FileOps;
using Stankins.Interfaces;
using Stankins.Interpreter;
using Stankins.SimpleRecipes;
using StankinsCommon;
using StankinsCronFiles;
using StankinsHelperCommands;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stankins.Console
{
    //https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
    internal class Program
    {
        private static string ExtendedHelpText()
        {
            string connectionString = "\"Server=..;Database=..;User Id=...;Password=...\"";
            //docker example
            //connectionString = "\"Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;\"";
            string nl = Environment.NewLine;
            StringBuilder sb = new StringBuilder();
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

        private static void WriteLines(ResultTypeStankins[] b)
        {
            foreach (ResultTypeStankins it in b)
            {
                Type t = it.Type;
                System.Console.WriteLine("");
                System.Console.Write($"stankins.console execute -o {t.Name}");

                foreach (KeyValuePair<string, object> item in it.ConstructorParam)
                {
                    System.Console.Write($" -a {item.Key}");
                }



            };
        }

        private static void Main(string[] args)
        {

            ResultTypeStankins[] refs = FindAssembliesToExecute.AddReferences();

            CtorDictionaryGeneric<ResultTypeStankins> commands = new CtorDictionaryGeneric<ResultTypeStankins>();

            Action<ResultTypeStankins> createItem = (t) =>
            {
                if (commands.ContainsKey(t.Name))
                {
                    System.Console.WriteLine($"key exists : {t.Name}");
                }
                else
                {
                    commands.Add(t.Name, t);
                }
            };
            foreach (ResultTypeStankins item in refs)
            {
                createItem(item);
            }


            CommandLineApplication app = new CommandLineApplication
            {
                Name = "Stankins.Console"
            };
            string versionString = Assembly.GetEntryAssembly()
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
            app.Command("recipes", (command) =>
            {
                command.Description = "execute/list recipes already in system";
                CommandOption list = command.Option("-l|--list", "list recipes", CommandOptionType.NoValue);
                CommandOption execute = command.Option("-e|--execute", "execute recipe", CommandOptionType.SingleValue);
                command.OnExecute(async () =>
                {
                    if (list.HasValue())
                    {
                        List<Recipe> recipes = RecipeFromString.RecipesFromFolder().ToList();
                        recipes.Sort((a, b) => a.Name.CompareTo(b.Name));
                        foreach (Recipe item in recipes)
                        {
                            System.Console.WriteLine($"Stankins.Console recipes -e {item.Name}");
                        }
                        return 0;
                    }
                    if (execute.HasValue())
                    {
                        string recipeString = execute.Value();
                        RecipeFromString recipe = RecipeFromString.FindRecipe(recipeString);
                        if (recipe == null)
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
                string names = string.Join(',', Enum.GetNames(typeof(WhatToList)));

                command.Description = "Explain arguments  for supported objects - could be " + names;

                command.HelpOption("-?|-h|--help");
                CommandOption optWhat = command.Option("-what", "what to list", CommandOptionType.SingleValue);



                command.OnExecute(() =>
                {
                    if (!optWhat.HasValue())
                    {
                        System.Console.WriteLine("please add -what " + names);
                        return -1;
                    }
                    WhatToList val = (WhatToList)(int)Enum.Parse(typeof(WhatToList), optWhat.Value(), true);

                    List<ResultTypeStankins> all = commands.Select(it => it.Value).ToList();

                    List<ResultTypeStankins> find = all.Where(it => val == (val & it.FromType())).ToList();
                    find.Sort((a, b) => a.Name.CompareTo(b.Name));

                    WriteLines(find.ToArray());


                    System.Console.WriteLine("");
                    System.Console.WriteLine("!for often used  commands , see -h option");
                    return 0;
                });

            });

            app.Command("files", (command) =>
            {
                command.Description = "Execute file ";
                command.HelpOption("-?|-h|--help");
                CommandOption opt = command.Option("-f", "execute file ", CommandOptionType.MultipleValue);
                command.OnExecute(async () =>
                {
                    if (!opt.HasValue())
                    {
                        System.Console.WriteLine("please add -f fileName");
                        return 0;
                    }
                    int lenValuesCount = opt.Values.Count;
                    for (int i = 0; i < lenValuesCount; i++)
                    {
                        string fileName = opt.Values[i];
                        System.Console.WriteLine($"executing {fileName}");
                        string text = await File.ReadAllTextAsync(fileName);
                        var r = new RecipeFromString(text);
                        await r.TransformData(null);
                    }
                    return 0;
                });


            });
            app.Command("cron", (command) =>
            {
                command.Description = "Execute CRON file uninterrupted ";
                command.HelpOption("-?|-h|--help");
                CommandOption opt = command.Option("-d", "directory with cron files", CommandOptionType.SingleValue);
                command.OnExecute(async () =>
                {
                    if (!opt.HasValue())
                    {
                        System.Console.WriteLine("please add -d directoryname");
                        return 0;
                    }
                    var dir = opt.Value();
                    var r = new RunCRONFiles(dir);
                    var ct = new CancellationTokenSource();
                    var token = ct.Token;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    r.StartAsync(token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    System.Console.WriteLine("press any key to shutdown");
                    var s = System.Console.ReadKey();
                    ct.Cancel();
                    await Task.Delay(3000);
                    return 0;
                });


            });
            app.Command("execute", (command) =>
                {
                    command.Description = "Execute multiple ";
                    command.HelpOption("-?|-h|--help");
                    CommandOption opt = command.Option("-o", "execute object", CommandOptionType.MultipleValue);
                    CommandOption argObjects = command.Option("-a", "arguments for the object", CommandOptionType.MultipleValue);
                    //var locationArgument = command.Argument("[location]", "The object to execute -see list command ", true);

                    command.OnExecute(async () =>
                    {
                        if (!opt.HasValue())
                        {
                            System.Console.WriteLine("see list command for objects");
                            return 0;
                        }


                        int argNr = 0;
                        int lenValuesCount = opt.Values.Count;
                        for (int i = 0; i < lenValuesCount; i++)
                        {

                            string item = opt.Values[i].ToLowerInvariant();
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

                            string item = opt.Values[value].ToLowerInvariant();
                            ResultTypeStankins cmd = commands[item];
                            object[] ctorArgs = null;
                            if (cmd.ConstructorParam.Count > 0)
                            {
                                ctorArgs = new object[cmd.ConstructorParam.Count];
                                int i = 0;
                                do
                                {
                                    string item1 = argObjects.Values[argNr];

                                    ctorArgs[i] = item1;
                                    i++;
                                    argNr++;

                                } while (i < cmd.ConstructorParam.Count);
                            }

                            last = cmd.Create(ctorArgs);
                            data = await last.TransformData(data);


                        }


                        ISenderToOutput output = last as ISenderToOutput;
                        if (output != null)
                        {
                            System.Console.WriteLine("exporting default output");
                            SenderOutputToFolder sender = new SenderOutputToFolder("", true);
                            data = await sender.TransformData(data);

                        }
                        else
                        {

                            System.Console.WriteLine("exporting all tables to csv");
                            await new SenderAllTablesToFileCSV("").TransformData(data);

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
