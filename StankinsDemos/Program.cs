using CommonDB;
using Logging;
using Microsoft.Extensions.Logging;
using MediaTransform;
using ReceiverDatabaseObjects;
using ReceiverFileSystem;
using ReceiverJob;
using ReceiverJSON;
using SenderBulkCopy;
using SenderHTML;
using SenderToDBSqlServer;
using SenderToFile;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformers;
using System.Threading;

namespace StankinsDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (StartLogging st = new StartLogging("asd", "Asda", 1))
            //{
            //    st.LogInformation("test'");
            //    Thread.Sleep(10);
            //}

            //return;
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
            di = Directory.CreateDirectory("Demo1JobFolders");
            file = "Demo1JobFolders.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Demo1SimpleJobFolders.html";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "Views/RazorRow.cshtml";
            overWriteFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo1);
            overWriteFile(file, Path.Combine(di.FullName, file));

            #endregion

            var strDemo2 = SimpleJobView(SimpleJobFolders(), "Demo2SimpleJobView.html");
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
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo2);
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
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo3);
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
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo4);
            overWriteFile(file, Path.Combine(di.FullName, file));

            #endregion

            #endregion

            #region PBX 
            var strDemo5 = PBXJob();
            File.WriteAllText("Demo5PBX.txt", strDemo5);
            //si = new SimpleJob();
            //si.UnSerialize(strDemo5);
            //si.Execute().GetAwaiter().GetResult();
            #region move into demos
            di = Directory.CreateDirectory("Demo5PBX");
            file = "Demo5PBX.txt";
            overWriteFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            overWriteFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo5);
            overWriteFile(file, Path.Combine(di.FullName, file));
            #endregion
            #endregion
            #region analysis project
            if (false)
            {
                ExecuteSlnAnalysis().GetAwaiter().GetResult();
                di = Directory.CreateDirectory("Demo6AnalysisProject");
                file = "Stankins.html";
                overWriteFile(file, Path.Combine(di.FullName, file));
                file = "StankinsNETFramework.html";
                overWriteFile(file, Path.Combine(di.FullName, file));
            }
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
            var folderSolution = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
            var receiveFolder = new ReceiverFolderHierarchical(folderSolution, "*.csproj");
            receiveFolder.ExcludeFolderNames = new string[] { "bin", "obj", "Properties", ".vs", ".git", "packages" };

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
        static string SimpleJobView(string contents, string output)
        {
            var si = new SimpleJob();

            string fileName = DeleteFileIfExists(output);
            File.WriteAllText("jobDefinition.txt", contents);
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
        static string ExecuteVisualizationDefinitionSimpleJob(string contents)
        {

            var si = new SimpleJob();
            si.UnSerialize(SimpleJobView(contents, "jobDefinition.html"));
            si.Execute().GetAwaiter().GetResult();
            return "jobDefinition.html";
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
            var filter = new FilterExcludeRelation(new string[] { "columns", "tables" });

            var si = new SimpleJobConditionalTransformers();
            si.Receivers.Add(0, rr);
            si.AddSender(sender);
            si.Add(filter);
            si.Add(filter, senderViz);
            return si.SerializeMe();
        }

        static string PBXJob()
        {

            var dir = AppContext.BaseDirectory;
            var dirPBX = Path.Combine(dir, "PBX");


            var serialize = new SerializeDataOnFile("a.txt");
            IReceive r = new ReceiverFolderHierarchical(dirPBX, "*.log");
            IFilter filterFiles = new FilterForFilesHierarchical();
            #region filter for remove dates serialize 

            var filterDateTime = new FilterComparableGreat(typeof(DateTime), DateTime.MinValue, "LastWriteTimeUtc");
            IFilter filterDateTimeSerializable = new FilterComparableFromSerializable(filterDateTime, serialize);
            #endregion

            IFilter removeFilesMaxWritten = new FilterRemovePropertyMaxMinDateTime("LastWriteTimeUtc", GroupingFunctions.Max);
            ITransform transformLines = new TransformerFileToLines() { TrimEmptyLines = true };
            var trDateRegex = new TransformRowRegex(@"^Date:\ (?<datePBX>.{23}).*?$", "text");
            var trToDate = new TransformerFieldStringToDate("datePBX", "NewDatePBX", "yyyy/MM/dd HH:mm:ss.fff");

            var trAddDate = new TransformAddFieldDown("NewDatePBX");
            var trSimpleFields = new TransformRowRemainsProperties("NewDatePBX", "lineNr", "text", "FullName", "LastWriteTimeUtc");

            var data = new DBTableDataConnection<SqlConnection>(new SerializeDataInMemory());
            data.ConnectionString = "#file:SqlServerConnectionString#";
            data.Fields = new string[] { "NewDatePBX", "lineNr", "text", "FullName" };
            data.TableName = "PBXData";
            var bulk = new SenderSqlServerBulkCopy(data);
            var md = new MediaTransformMaxMin<DateTime>();
            md.GroupFunction = GroupingFunctions.Max;
            md.FieldName = "LastWriteTimeUtc";
            var serializeMaxDate = new SenderMediaSerialize<DateTime>(serialize, "LastWriteTimeUtc", md);
            var si = new SimpleJob();
            si.Receivers.Add(0, r);
            int iFilterNr = 0;
            si.FiltersAndTransformers.Add(iFilterNr++, filterFiles);
            si.FiltersAndTransformers.Add(iFilterNr++, filterDateTimeSerializable);
            si.FiltersAndTransformers.Add(iFilterNr++, removeFilesMaxWritten);
            si.FiltersAndTransformers.Add(iFilterNr++, transformLines);
            si.FiltersAndTransformers.Add(iFilterNr++, trDateRegex);
            si.FiltersAndTransformers.Add(iFilterNr++, trToDate);
            si.FiltersAndTransformers.Add(iFilterNr++, trAddDate);
            si.FiltersAndTransformers.Add(iFilterNr++, trSimpleFields);
            //TODO: add transformer to add a field down for all fields
            //TODO: add transformer regex for splitting Key=Value
            //TODO: add field to separate Conn(1)Type(Any)User(InternalTask) CDL Request:RSVI(Get)
            si.Senders.Add(0, bulk);
            si.Senders.Add(1, serializeMaxDate);

            return si.SerializeMe();

        }

        static async Task ExecuteSlnAnalysis()
        {
            string root = "@static:Path.GetPathRoot(#static:Directory.GetCurrentDirectory()#)@";
            var si=new SimpleJob();
            var recFolder=new ReceiverFolderHierarchical(root, "*tank*.sln;*StankinsSimpleJobNET*.exe");
            si.Receivers.Add(0, recFolder);
            IFilter fi = new FilterForFilesHierarchical();
            si.FiltersAndTransformers.Add(0, fi);

            si.UnSerialize(si.SerializeMe());
            await si.Execute();
            fi = si.FiltersAndTransformers[0] as IFilter;
            var exe = fi.valuesTransformed.FirstOrDefault(it => it.Values["FullName"].ToString().EndsWith(".exe"));
            if(exe == null)
            {
                Console.WriteLine("please build StankinsSimpleJobNET");
                return;
            }
            string exePath = exe.Values["FullName"].ToString();
            string exeDir = Path.GetDirectoryName(exePath);
            //cleanup
            foreach (var item in Directory.GetFiles(exeDir, "*.json"))
                File.Delete(item);

            //File.Copy("SolutionExport.txt", Path.Combine(exeDir, "SolutionExport.txt"));
            var slns = fi.valuesTransformed.Select(it=>it.Values["FullName"]?.ToString()).Where(it => (it??"").Contains(".sln")).ToArray();
            foreach(var sln in slns)
            {
                Console.WriteLine($"interpret:{sln}");
                var psi = new ProcessStartInfo(exePath, "execute SolutionExport.txt");
                psi.WorkingDirectory = exeDir;
                psi.Environment["solutionPath"] = sln;
                var p = Process.Start(psi);
                if (p.WaitForExit(60 * 1000))
                {
                    var newJob = new SimpleJobConditionalTransformers();
                    var json = Path.Combine(exeDir, Path.GetFileNameWithoutExtension(sln) + ".json");
                    var rec = new ReceiverJSONFileInt(json, Encoding.UTF8);
                    newJob.Receivers.Add(0, rec);

                    string fileName =Path.Combine(AppContext.BaseDirectory, Path.GetFileNameWithoutExtension(sln) + ".html");
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    Console.WriteLine($"exporting to {fileName}");

                    string fileRazor = "solution.cshtml";
                    var sender= new SyncSenderMultiple(
                        new Sender_HTMLText(fileName, "<html><body>"),
                        new Sender_HTMLRazor("Views/" + Path.GetFileName(fileRazor), fileName)
                        

                     );
                    newJob.AddSender(sender);
                    var filter = new FilterExcludeRelation("referencedIn", "assemblyReferenced");
                    

                    var senderViz = new SyncSenderMultiple(
                    new Sender_HTMLRelationViz("Name", fileName),
                        new Sender_HTMLText(fileName, "</body></html>")
                        );

                    newJob.Add(filter, senderViz);
                    
                    await newJob.Execute();
                }
            }


        }
    }
}
