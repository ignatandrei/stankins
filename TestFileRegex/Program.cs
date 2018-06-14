using ReceiverFileSystem;
using SenderToFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Transformers;

namespace TestFileRegex
{
    class Program
    {
        static  void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var sj = new SimpleJob();

            IReceive folder = new ReceiverFolderHierarchical(@"D:\test", "*.txt");
            //await folder.LoadData();
            var t = new TransformerFileToLines();
            t.TrimEmptyLines = false;
            //t.valuesRead = folder.valuesRead;
            //await t.Run();

            var addBak = new TransformerFieldAddString("FullName", "FullName", ".bak");
            addBak.valuesRead = t.valuesTransformed;
            //await addBak.Run();
            

            //var regex = new TransformRowRegex(@"^(?x<ip>123).*?$","text");
            var regex = new TransformRowRegexReplaceGuid(@"^.*x(?<ip>\w+)123.*?$", "text");
            regex.ReplaceAllNextOccurences = true;
            //regex.valuesRead = addBak.valuesTransformed;
            //await regex.Run();
            var file = new SenderByRowToFile("FullName", "lineNr", "text",".txt");
            
            file.FileMode = System.IO.FileMode.Append;
            //file.valuesToBeSent = regex.valuesTransformed;
            //await file.Send();

            sj
                .AddReceiver(folder)
                .AddTransformer(t)
                //.AddTransformer(addBak)
                .AddTransformer(regex)
                .AddSender(file);

            File.WriteAllText("def.txt", sj.SerializeMe());
            await sj.Execute();



        }

    }
}
