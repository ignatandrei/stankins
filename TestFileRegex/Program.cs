using ReceiverFileSystem;
using SenderToFile;
using StankinsInterfaces;
using System;
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

            IReceive folder = new ReceiverFolderHierarchical(@"D:\test", "*.txt");
            await folder.LoadData();
            var t = new TransformerFileToLines();
            t.TrimEmptyLines = false;
            t.valuesRead = folder.valuesRead;
            await t.Run();

            var addBak = new TransformerFieldAddString("FullName", "FullName", ".bak");
            addBak.valuesRead = t.valuesTransformed;
            await addBak.Run();
            

            //var regex = new TransformRowRegex(@"^(?x<ip>123).*?$","text");
            var regex = new TransformRowRegexReplaceGuid(@"^.*x(?<ip>\w+)123.*?$", "text");
            regex.valuesRead = addBak.valuesTransformed;
            await regex.Run();
            Console.WriteLine(regex.valuesTransformed.Length);
            var file = new SenderByRowToFile("FullName", ".txt", "text");
            file.valuesToBeSent = regex.valuesTransformed;
            file.FileMode = System.IO.FileMode.OpenOrCreate;
            await file.Send();
            


        }

    }
}
