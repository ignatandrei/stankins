using ReceiverFileSystem;
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
            
            var strJobFolders = SimpleJobFolders();
            //if you want you can execute with StankinsSimpleJob
            //string file = Path.Combine(dir,"jobFolders.txt");
            //File.WriteAllText(file, SimpleJobFolders());
            //Console.WriteLine($"executing file {file}");

            var si = new SimpleJob();
            si.UnSerialize(strJobFolders);
            si.Execute().GetAwaiter().GetResult();
            //Process.Start("StankinsSimpleJob.exe", "execute -fileWithSimpleJob " + file).WaitForExit();
            
            

        }
        
        static string SimpleJobFolders()
        {
            string fileName = "Demo1Folder.html";
            //TODO: put this on interpret string , like env
            
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
