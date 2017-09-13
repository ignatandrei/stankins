using ReceiverFileSystem;
using ReceiverJob;
using SenderHTML;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Diagnostics;
using System.IO;

namespace StankinsDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            string dir = Directory.GetCurrentDirectory();


            //if you want you can execute with StankinsSimpleJob
            //string file = Path.Combine(dir,"jobFolders.txt");
            //File.WriteAllText(file, SimpleJobFolders());
            //Console.WriteLine($"executing file {file}");
            IJob si;
            var strDemo1 = SimpleJobFolders();
            File.WriteAllText("Demo1JobFolders.txt", strDemo1);
            si = new SimpleJob();
            si.UnSerialize(strDemo1);
            si.Execute().GetAwaiter().GetResult();
            
            var strDemo2 = SimpleJobView();
            File.WriteAllText("Demo2JobView.txt", strDemo2);
            si = new SimpleJob();
            si.UnSerialize(strDemo2);
            si.Execute().GetAwaiter().GetResult();

        }
        static string DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            return fileName;
        }
        static string SimpleJobView()
        {
            var si = new SimpleJob();

            string fileName = DeleteFileIfExists("Demo2SimpleJobView.html");
            File.WriteAllText("jobDefinition.txt", SimpleJobFolders());
            var receiver = new ReceiverFromJobFile("jobDefinition.txt");
            si.Receivers.Add(0,receiver);
            si.Senders.Add(0, new Sender_HTMLText(fileName, "<html><body><h1>Job visualization</h1>"));
            si.Senders.Add(1, new Sender_HTMLRazor("Views/RazorRow.cshtml", fileName));
            si.Senders.Add(2, new Sender_HierarchicalVizJob(fileName, "Name"));
            si.Senders.Add(3, new Sender_HTMLText(fileName, "</body></html>"));
            //or you can add SyncSenderMultiple , but for now let's do it line by line
            //ISend sender = new SyncSenderMultiple(
            //    new Sender...
            //    new Sender...
            //    new Sender...
            //    new Sender...
            //    );
            return si.SerializeMe();
        }
        static string SimpleJobFolders()
        {
            string fileName = DeleteFileIfExists("Demo1SimpleJobFolders.html");
            //TODO: put current dir on interpret string , like env
            
            var folderSolution= new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
            var receiveFolder = new ReceiverFolder(folderSolution, "*.csproj");
            receiveFolder.ExcludeFolderNames = new string[] { "bin", "obj","Properties" ,".vs",".git","packages" };

            var si = new SimpleJob();
            si.Receivers.Add(0, receiveFolder);
            si.Senders.Add(0, new Sender_HTMLText(fileName, "<html><body><h1>Find .csproj in solution folder</h1>"));
            si.Senders.Add(1, new Sender_HTMLRazor("Views/RazorHierarchical.cshtml", fileName));
            si.Senders.Add(2, new Sender_HierarchicalVizFolder(fileName, "Name"));
            si.Senders.Add(3, new Sender_HTMLText(fileName, "</body></html>"));
            //or you can add SyncSenderMultiple , but for now let's do it line by line
            //ISend sender = new SyncSenderMultiple(
            //    new Sender_HTMLText(fileName, "<html><body>"),
            //    new Sender_HTMLRazor("Views/RazorHierarchical.cshtml", fileName),
            //    new Sender_HierarchicalVizFolder(fileName, "Name"),
            //    new Sender_HTMLText(fileName, "</body></html>")
            //    );
            return si.SerializeMe();
        }
    }
}
