using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using StankinsInterfaces;
using StankinsSimpleFactory;
using StanskinsImplementation;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace StankinsSimpleJob
{
    class Program
    {
        enum AddDataSimpleJob
        {
            ExitChoice= 0,
            AddReceiver=1,
            AddTransformer=2,
            AddSender=3
        }

        static void Main(string[] args)
        {
            if (args?.Length == 0)
            {
                args = new string[] { "-h" };
            }
            //TODO: load assemblies on demand
            var app = new CommandLineApplication(throwOnUnexpectedArg: true);
            app.Name = Assembly.GetEntryAssembly().GetName().Name;
            app.HelpOption("-?|-h|--help");

            app.Command("generate", (cmd) => {
                cmd.HelpOption("-?|-h|--help");
                cmd.OnExecute(() =>
                {
                    return GenerateJobDefinition();
                });
            });
            app.Command("execute",(cmd)=>{
                cmd.HelpOption("-?|-h|--help");
                var file = cmd.Argument("fileWithSimpleJob", "file serialized as simple job");
                cmd.OnExecute(() =>
                {
                    var settings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented,
                        Error = HandleDeserializationError
                        //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

                    };
                    var valSerialized = File.ReadAllText(file.Value);
                    var deserialized = JsonConvert.DeserializeObject(valSerialized, settings) as ISimpleJob;
                    //TODO:log
                    deserialized.Execute().Wait();
                    return 0;
                });
            });
            app.Execute(args);
            
        }
        private static IReceive GetReceiver(SimpleJobFactory factory)
        {
            int i = 1;
            foreach (var receiver in factory.ReceiverNames())
            {
                Console.WriteLine($"{i++}){receiver}");
            }
            Console.Write("Please enter receiver id");
            var nrReceiver = int.Parse(Console.ReadLine());

            var receiverNameSelected = factory.ReceiverNames()[nrReceiver - 1];
            return factory.GetReceiver(receiverNameSelected);
        }
        private static int GenerateJobDefinition()
        {
            var factory = new SimpleJobFactory();
            int i = 1;
            var jobNames = factory.JobNames();
            foreach (var jobName in jobNames)
            {
                Console.WriteLine($"{i++}){jobName}");
            }
            Console.Write("Please enter job id");
            var nrJob = int.Parse(Console.ReadLine());
            var jobNameSelected= jobNames[nrJob - 1];
            //
            i = 1;
            
            var job = factory.GetJob(jobNameSelected);
            var choice = AddDataSimpleJob.ExitChoice;
            do
            {
                Console.WriteLine($"{(int)AddDataSimpleJob.ExitChoice}. Exit");
                Console.WriteLine($"{(int)AddDataSimpleJob.AddReceiver}. Add Receiver");
                Console.WriteLine($"{(int)AddDataSimpleJob.AddTransformer}. Add transformer");
                Console.WriteLine($"{(int)AddDataSimpleJob.AddSender}. Add Sender");
                choice = (AddDataSimpleJob)int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case AddDataSimpleJob.ExitChoice:
                        break;
                    case AddDataSimpleJob.AddReceiver:
                        var rec = GetReceiver(factory);
                        job.Receivers.Add(job.Receivers.Count, rec);
                        //TODO: choice for receiver properties: connection string , ...
                        break;
                    default:
                        Console.WriteLine("not yet implemented");
                        break;
                }
            } while (choice != AddDataSimpleJob.ExitChoice);



            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            
            var serialized = JsonConvert.SerializeObject(job, settings);
            File.WriteAllText("a.txt", serialized);
            Process.Start("notepad.exe", "a.txt");
            return 0;
        }

        //TODO: log
        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            var currentError = e.ErrorContext.Error.Message;
            e.ErrorContext.Handled = true;

        }
    }
}