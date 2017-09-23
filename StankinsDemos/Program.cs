using CommonDB;
using ReceiverDatabaseObjects;
using ReceiverFileSystem;
using ReceiverJob;
using SenderHTML;
using SenderToDBSqlServer;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Transformers;

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
            DirectoryInfo di;
            string file;
            Action<string, string> overWriteFile = (fileName, fileDestination) =>
             {

                 if (File.Exists(fileDestination))
                     File.Delete(fileDestination);
                 string destDir = Path.GetDirectoryName(fileDestination);
                 if (!Directory.Exists(destDir))
                     Directory.CreateDirectory(destDir);

                 File.Copy(fileName, fileDestination);
             };
            #region move into demos
            di=Directory.CreateDirectory("Demo1JobFolders");
            file = "Demo1JobFolders.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Demo1SimpleJobFolders.html";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/RazorRow.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            #endregion

            var strDemo2 = SimpleJobView();
            File.WriteAllText("Demo2JobView.txt", strDemo2);
            si = new SimpleJob();
            si.UnSerialize(strDemo2);
            si.Execute().GetAwaiter().GetResult();
            #region move into demos
            di = Directory.CreateDirectory("Demo2SimpleJobView");
            file = "jobDefinition.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Demo2JobView.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Demo2SimpleJobView.html";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/RazorHierarchical.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));

            #endregion
            var strDemo3 = ExecuteSqlCIOrder();
            File.WriteAllText("Demo3ExecuteSql.txt", strDemo3);
            si = new SimpleJob();
            si.UnSerialize(strDemo3);
            si.Execute().GetAwaiter().GetResult();
            #region move into demos
            di = Directory.CreateDirectory("Demo3ExecuteSql");
            file = "Demo3ExecuteSql.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "SqlToExecute/001Sql.sql";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "SqlToExecute/002Sql.sql";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            overWriteFile(file, Path.Combine(di.FullName, file));
            #endregion
            #region DocumentSqlServer
            //TODO:add demo DocumentSqlServer
            var strDemo4 = DocumentSqlServer();
            File.WriteAllText("Demo4DocumentSqlServer.txt", strDemo3);
            si = new SimpleJobConditionalTransformers();
            si.UnSerialize(strDemo4);
            si.Execute().GetAwaiter().GetResult();
            #region move into demos
            di = Directory.CreateDirectory("Demo4DocumentSqlServer");
            file = "Demo4DocumentSqlServer.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/sqlserver.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/databases.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/tables.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/columns.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "relationalSqlServer.html";
            overWriteFile(file, Path.Combine(di.FullName, file));            
            file = "appsettings.json";
            overWriteFile(file, Path.Combine(di.FullName, file));
            #endregion

            #endregion



        }
        static string DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            return fileName;
        }
        
        static string SimpleJobFolders()
        {
            string fileName = DeleteFileIfExists("Demo1SimpleJobFolders.html");
            //TODO: put current dir on interpret string , like env            
            var folderSolution= new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
            var receiveFolder = new ReceiverFolderHierarchical(folderSolution, "*.csproj");
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
        static string SimpleJobView()
        {
            var si = new SimpleJob();

            string fileName = DeleteFileIfExists("Demo2SimpleJobView.html");
            File.WriteAllText("jobDefinition.txt", SimpleJobFolders());
            var receiver = new ReceiverFromJobFile("jobDefinition.txt");
            si.Receivers.Add(0, receiver);
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
        static string ExecuteSqlCIOrder()
        {
            string fileName = DeleteFileIfExists("Demo3SimpleExecuteSql.html");
            //TODO: put current dir on interpret string , like env            
            
            var receiveFolder = new ReceiverFolderHierarchical(@"#static:Directory.GetCurrentDirectory()#\SqlToExecute", "*.sql");
            var dbConnection = new DBDataConnection<SqlConnection>();
            dbConnection.ConnectionString = "#file:SqlServerConnectionString#";
            var senderSql = new SenderSqlToDBSqlServer(dbConnection);
            var si = new SimpleJob();
            si.Receivers.Add(0, receiveFolder);
            si.Senders.Add(0, senderSql);
            return si.SerializeMe();
        }
        static string DocumentSqlServer()
        {
            var rr = new ReceiverRelationalSqlServer();
            rr.ConnectionString = "#file:SqlServerConnectionString#";
            string OutputFileName = "relationalSqlServer.html";
            var sender = new Sender_HTMLRazor("Views/sqlserver.cshtml", OutputFileName);

            var senderViz = new Sender_HTMLRelationViz("Name", OutputFileName);
            var filter = new FilterExcludeRelation();
            filter.ExcludeProperties
                .AddRange(new string[] { "columns", "tables" });
            var si = new SimpleJobConditionalTransformers();
            si.Receivers.Add(0, rr);
            si.AddSender(sender);
            si.Add(filter);
            si.Add(filter, senderViz);
            return si.SerializeMe();
        }
    }
}
