using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using StankinsInterfaces;
using StankinsSimpleFactory;
using StanskinsImplementation;
using System;
using System.IO;
using System.Reflection;

namespace StankinsSimpleJob
{
    class Program
    {
        static void Main(string[] args)
        {
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
            var job = factory.GetJob(jobNameSelected);
            i = 1;
            foreach (var receiver in factory.ReceiverNames())
            {
                Console.WriteLine($"{i++}){receiver}");
            }
            Console.Write("Please enter receiver id");
            var nrReceiver = int.Parse(Console.ReadLine());
            var receiverNameSelected = factory.ReceiverNames()[nrReceiver - 1];
            var rec = factory.GetReceiver(receiverNameSelected);
            job.Receivers.Add(0,rec);
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };
            //TODO: use .NET Dependency injection for ISerializeData and other parameters
            var serialized = JsonConvert.SerializeObject(job, settings);
            File.WriteAllText("a.txt", serialized);
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