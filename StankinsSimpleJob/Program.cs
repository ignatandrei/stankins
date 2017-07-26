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
            foreach (var item in factory.JobNames())
            {
                Console.WriteLine($"{i}){item}");
            }
            Console.Write("Please enter job id");
            var nrJob = int.Parse(Console.ReadLine());
            var jobName = factory.JobNames()[nrJob - 1];

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