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
using ReceiverDll;
using ReceiverFile;
using TransformerHtmlUrl;

namespace StankinsDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
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
            DirectoryInfo di = null;
            string file;
            Action<string, string> moveFile = (fileNameSource, fileDestination) =>
            {
                string destDir = Path.GetDirectoryName(fileDestination);

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                if (File.Exists(fileDestination))
                    File.Delete(fileDestination);

                
                File.Move(fileNameSource, fileDestination);
            };
            Action<string, string> copyFile = (fileNameSource, fileDestination) =>
            {
                string destDir = Path.GetDirectoryName(fileDestination);

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                if (File.Exists(fileDestination))
                    File.Delete(fileDestination);


                File.Copy(fileNameSource, fileDestination);
            };
            Action<string, string,string> moveFiles = (path,search, pathDestination) =>
            {
               

                var files = Directory.EnumerateFiles(path, search, SearchOption.TopDirectoryOnly);
                foreach(var fileLoop in files)
                {
                    var name = Path.GetFileName(fileLoop);
                    moveFile(fileLoop, Path.Combine(pathDestination, name));
                }
                
            };
//           


            goto andrei;
            var strDemo1 = SimpleJobFolders();
            File.WriteAllText("jobDefinition.txt", strDemo1);
            si = new SimpleJob();
            si.UnSerialize(strDemo1);
            await si.Execute();


            #region move into demos
            di = Directory.CreateDirectory("Demo1JobFolders");
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Demo1SimpleJobFolders.html";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/RazorHierarchical.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo1);
            moveFile(file, Path.Combine(di.FullName, file));

            #endregion

            var strDemo2 = SimpleJobView(SimpleJobFolders(), "Demo2SimpleJobView.html");
            File.WriteAllText("jobDefinition.txt", strDemo2);
            si = new SimpleJob();
            si.UnSerialize(strDemo2);
            await si.Execute();
            #region move into demos
            di = Directory.CreateDirectory("Demo2SimpleJobView");
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Demo2SimpleJobView.html";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/RazorHierarchical.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo2);
            moveFile(file, Path.Combine(di.FullName, file));

            #endregion
            var strDemo3 = ExecuteSqlCIOrder();
            File.WriteAllText("jobDefinition.txt", strDemo3);
            si = new SimpleJob();
            si.UnSerialize(strDemo3);
            await si.Execute();
            #region move into demos
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            di = Directory.CreateDirectory("Demo3ExecuteSql");
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "SqlToExecute/001Sql.sql";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "SqlToExecute/002Sql.sql";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            copyFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo3);
            moveFile(file, Path.Combine(di.FullName, file));
            #endregion
            #region DocumentSqlServer
            //TODO:add demo DocumentSqlServer
            var strDemo4 = DocumentSqlServer();
            File.WriteAllText("jobDefinition.txt", strDemo3);
            si = new SimpleJobConditionalTransformers();
            si.UnSerialize(strDemo4);
            await si.Execute();
            #region move into demos
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            di = Directory.CreateDirectory("Demo4DocumentSqlServer");
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/sqlserver.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/databases.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/tables.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/views.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "Views/columns.cshtml";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "relationalSqlServer.html";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            copyFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo4);
            moveFile(file, Path.Combine(di.FullName, file));

            #endregion

            #endregion

            #region PBX 
            var strDemo5 = PBXJob();
            File.WriteAllText("jobDefinition.txt", strDemo5);
            si = new SimpleJob();
            si.UnSerialize(strDemo5);
            try
            {
                await si.Execute();
            }
            catch (InvalidOperationException)
            {

                //do nothing - sql server does not have table
            }

            #region move into demos
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            di = Directory.CreateDirectory("Demo5PBX");
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            copyFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo5);
            moveFile(file, Path.Combine(di.FullName, file));
            #endregion
            #endregion
            #region analysis project
            if (false)
            {
                await ExecuteSlnAnalysis();
                di = Directory.CreateDirectory("Demo6AnalysisProject");
                file = "Stankins.html";
                moveFile(file, Path.Combine(di.FullName, file));
                file = "StankinsNETFramework.html";
                moveFile(file, Path.Combine(di.FullName, file));
            }
            #endregion

            #region showDllTypes
            var strDemo7 = SimpleJobDllLoadTypes();
            di = Directory.CreateDirectory("Demo7LoadDllTypes");
            File.WriteAllText("jobDefinition.txt", strDemo7);
            si = new SimpleJob();
            si.UnSerialize(strDemo7);
            await si.Execute();
            #region move into demos
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "relationalDLL.html";
            moveFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            file = ExecuteVisualizationDefinitionSimpleJob(strDemo7);
            moveFile(file, Path.Combine(di.FullName, file));
            #endregion
            #endregion
           
            andrei:
            #region blockly table

            var strDemo8 = JobDllBlockly();
            di = Directory.CreateDirectory("Demo8Blockly");
            if (File.Exists("jobDefinition.txt"))
                File.Delete("jobDefinition.txt");
            File.WriteAllText("jobDefinition.txt", strDemo8);
            si = new SimpleJobConditionalTransformers();
            si.UnSerialize(strDemo8);
            await si.Execute();
            #region move into demos
            file = "readme.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "jobDefinition.txt";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "appsettings.json";
            copyFile(file, Path.Combine(di.FullName, file));
            file = "blockly.html";
            moveFile(file, Path.Combine(di.FullName, file));
            //execute visualization
            var dirBlocks = Path.Combine(di.FullName, "blocks");
           moveFiles(Directory.GetCurrentDirectory(), "*block*.*", dirBlocks);
           file = ExecuteVisualizationDefinitionSimpleJob(strDemo8);
            moveFile(file, Path.Combine(di.FullName, file));
            #endregion
            #endregion
            

        }
        static string DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            return fileName;
        }
        static string SimpleJobDllLoadTypes()
        {
            string dir = Environment.CurrentDirectory;
            IReceive folderWithDll = new ReceiverFolderHierarchical("#static:Directory.GetCurrentDirectory()#", "*.dll");
            var filterFiles = new FilterForFilesHierarchical();
            var recContentFile = new ReceiverFromDllRelational();
            var loadDllromFiles = new TransformerApplyReceiver(recContentFile, "DllFileName", "FullName");

            var fileRazor = Path.Combine(dir, "relationalGeneric.cshtml");
            string filename = "#static:Directory.GetCurrentDirectory()#\\relationalDLL.html";            
            ISend senderHTML = new SyncSenderMultiple(
                new Sender_Text(filename, "<html><body>") { FileMode = FileMode.Create},
                new Sender_HTMLRazor("Views/" + Path.GetFileName(fileRazor), filename),
                //new Sender_HTMLRelationViz("Name", filename),
                new Sender_Text(filename, "</body></html>")
                )
                ;
            var job = new SimpleJob();
            job.Receivers.Add(0, folderWithDll);
            job.FiltersAndTransformers.Add(0, filterFiles);
            job.FiltersAndTransformers.Add(1, loadDllromFiles);
            job.Senders.Add(0, senderHTML);
            return job.SerializeMe();

        }
        static string JobDllBlockly()
        {
            string dir = Environment.CurrentDirectory;
            IReceive folderWithDll = new ReceiverFolderHierarchical("#static:Directory.GetCurrentDirectory()#", "*.dll");
            var filterFiles = new FilterForFilesHierarchical();
            var recContentFile = new ReceiverFromDllRelational();
            var loadDllFromFiles = new TransformerApplyReceiver(recContentFile, "DllFileName", "FullName");
            //retain just types
            var filterTypes = new FilterRetainRelationalName(FilterType.Equal, "Types");
            //load types that are not generic or abstract
            //var justTypes = new FilterComparableEqual(typeof(string), "Types", "NameRelation");
            var notAbstract = new FilterComparableEqual(typeof(bool), false, "IsAbstract");
            var notGeneric = new FilterComparableEqual(typeof(bool), false, "IsGeneric");
            var notInterface = new FilterComparableEqual(typeof(bool), false, "IsInterface");
            //var noInterface = new FilterExcludeRelation("Interfaces");
            //var rel2plain = new TransformerRelationalToPlain();
            var haveProps = new FilterComparableGreat(typeof(int), 0, "PropertiesNr");
            //var justRelations = new FilterRetainItemsWithKey("NameRelation", FilterType.Equal);
            
            var fileRazor = Path.Combine(dir, "blockly.cshtml");
            string filename = "#static:Directory.GetCurrentDirectory()#\\blockly.html";
            ISend senderHTML = new SyncSenderMultiple(
                new Sender_Text(filename, "<html><body>") { FileMode = FileMode.Create },
                new Sender_HTMLRazor("Views/" + Path.GetFileName(fileRazor), filename),
                //new Sender_HTMLRelationViz("Name", filename),
                new Sender_Text(filename, "</body></html>")
                )
                ;
            var senderRow = new SenderByRowToFile("Name", "txt", "Block Definition");
            var clear = new TransformClearValues();
            var trReceiveHtml=new ReceiverHTMLTable(@"blockly.html", Encoding.UTF8);
            var decodeHtml = new TransformerHtmlDecode();
            //var addJS = new TransformModifyField("Name", "{0}.js");
            var senderBlockTag = new SenderByRowToFile("Name", "blockDefinition.txt", "Block definition");
            var senderBlockDefinitionJs= new SenderByRowToFile("Name", "blockDefinition.js", "Block JS");
            var senderBlockCodeJs = new SenderByRowToFile("Name", "blockCodeJavascript.js", "Block CodeGenerator");
            
            var job = new SimpleJobConditionalTransformers();
            job
            .AddReceiver(folderWithDll)
            .AddFilter(filterFiles)
            .AddTransformer(loadDllFromFiles)
            .AddFilter(filterTypes)
            .AddFilter(notAbstract)
            .AddFilter(notGeneric)
            .AddFilter(notInterface)
            .AddFilter(haveProps)
            //.AddTransformer(noInterface)            
            //.AddTransformer(rel2plain)
            .AddSender(senderHTML)
            .AddTransformer(clear)
            .Add(clear, trReceiveHtml) 
            .Add(trReceiveHtml, decodeHtml)
            .Add(decodeHtml, senderBlockTag)
            .Add(decodeHtml, senderBlockDefinitionJs)
            .Add(decodeHtml, senderBlockCodeJs)
            
            ;

            return job.SerializeMe();

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
            si.Senders.Add(0, new Sender_Text(fileName, "<html><body><h1>Find .csproj in solution folder</h1>"));
            si.Senders.Add(1, new Sender_HTMLRazor("Views/RazorHierarchical.cshtml", fileName));
            si.Senders.Add(2, new Sender_HierarchicalVizFolder(fileName, "Name"));
            si.Senders.Add(3, new Sender_Text(fileName, "</body></html>"));
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
            si.Senders.Add(0, new Sender_Text(fileName, "<html><body><h1>Job visualization</h1>"));
            si.Senders.Add(1, new Sender_HTMLRazor("Views/RazorRow.cshtml", fileName));
            si.Senders.Add(2, new Sender_HierarchicalVizJob(fileName, "Name"));
            si.Senders.Add(3, new Sender_Text(fileName, "</body></html>"));
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
            sender.FileMode = FileMode.Create;
            var filter = new FilterExcludeRelation(new string[] { "columns", "tables" ,"views"});
            var senderViz = new Sender_HTMLRelationViz("Name", OutputFileName);
            

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
            var trKeyValueRegex = new TransformRowRegex(@"(^|\r|\n|\r\n)(?<tkey>(\$?[a-zA-Z0-9_[\]/ ]+))='?(?<tvalue>([a-zA-Z0-9_ ]+))'?(\r|\r\n|$)", "text");
            var trToDate = new TransformerFieldStringToDate("datePBX", "NewDatePBX", "yyyy/MM/dd HH:mm:ss.fff");

            var trAddDate = new TransformAddFieldDown("NewDatePBX");
            var trAddKey = new TransformAddNewField("tkey");
            var trAddValue = new TransformAddNewField("tvalue");

            var trSimpleFields = new TransformRowRemainsProperties("NewDatePBX", "lineNr", "text", "FullName", "LastWriteTimeUtc", "tkey", "tvalue");

            var data = new DBTableDataConnection<SqlConnection>(new SerializeDataInMemory());
            data.ConnectionString = "#file:SqlServerConnectionString#";
            data.Fields = new string[] { "NewDatePBX", "lineNr", "text", "FullName", "tkey", "tvalue" };
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
            si.FiltersAndTransformers.Add(iFilterNr++, trKeyValueRegex);
            si.FiltersAndTransformers.Add(iFilterNr++, trToDate);
            si.FiltersAndTransformers.Add(iFilterNr++, trAddDate);
            si.FiltersAndTransformers.Add(iFilterNr++, trAddKey);
            si.FiltersAndTransformers.Add(iFilterNr++, trAddValue);
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
                        new Sender_Text(fileName, "<html><body>"),
                        new Sender_HTMLRazor("Views/" + Path.GetFileName(fileRazor), fileName)
                        

                     );
                    newJob.AddSender(sender);
                    var filter = new FilterExcludeRelation("referencedIn", "assemblyReferenced");
                    

                    var senderViz = new SyncSenderMultiple(
                    new Sender_HTMLRelationViz("Name", fileName),
                        new Sender_Text(fileName, "</body></html>")
                        );

                    newJob.Add(filter, senderViz);
                    
                    await newJob.Execute();
                }
            }


        }
    }
}
