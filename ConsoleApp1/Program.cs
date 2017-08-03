using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.IO.MemoryMappedFiles;
using ReceiverDB;
using ReceiverDBSqlServer;
using SenderCSV;
using SenderJSON;
using SenderXML;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SenderDBInflux;
using System.Reflection;
using Transformers;
using ReceiverBookmarkExportChrome;
using SenderHTML;
using SenderElasticSearch;
//using Transformers.BasicTransformersType;
//using Aq.ExpressionJsonSerializer;
//using Microsoft.Data.Sqlite;
namespace ConsoleApp1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            //var f = new TransformerIntString("asd", "bas");

            //var settings1 = new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    Formatting = Formatting.Indented,
            //    Error = HandleDeserializationError
            //    //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            //};
            //settings1.Converters.Add(new ExpressionJsonConverter(Assembly.GetEntryAssembly()));
            //var x = JsonConvert.SerializeObject(f,settings1);
            //Console.WriteLine(x);
            //return;
            brrbrrrr();return;

            var receiver = new ReceiverBookmarkFileChrome(@"C:\Users\admin\Documents\bookmarks_7_25_17.html");
            //var tr = new TransformAddField<string, DateTime>(
            //    (addDate) =>
            //    {
            //        var secs = double.Parse(addDate);
            //        return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(secs);
            //    }
            //    , "ADD_DATE", "realDate");
            var tr = new TransformAddField<string, DateTime>(
                (addDate) => 
                new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddSeconds(double.Parse(addDate))
                    , "ADD_DATE", "realDate");
            var sender = new SenderToHTML("BKChrome.cshtml", "b.html");

            SimpleJob sj = new SimpleJob();
            sj.Receivers.Add(0, receiver);
            sj.FiltersAndTransformers.Add(0, tr);
            sj.Senders.Add(0, sender);
            sj.Execute().Wait();
            return;
            //var x = MemoryMappedFile.CreateNew("testmap", 2);

            //using (var writer = x.CreateViewAccessor(0,200, MemoryMappedFileAccess.Write))
            //{
            //    // Write to MMF
            //    for(int s = 0; s < 20; s++)
            //    {
            //        Console.WriteLine(s);
            //        writer.Write(s, ('a'+s));
            //    }
            //}
            //using (var reader = x.CreateViewAccessor(0, 2000, MemoryMappedFileAccess.Read))
            //{
            //    // Write to MMF
            //    for (int s = 0; s < 20; s++)
            //    {
            //        Console.WriteLine(s);
            //        var d= reader.ReadByte(s);
            //        Console.WriteLine((char)d);
            //    }
            //}
            //return;

            ISerializeData sd = new SerializeDataInMemory();
            ISend csvExport = new SenderToCSV("a.csv");
            ISend xmlExport = new Sender_XML("a.xml","values");
            ISend jsonExport = new Sender_JSON("a.json");
            //ISend influx = new SenderDB_Influx("http://localhost:8086", "mydb", "logical_reads", "cpu_time_ms", "total_elapsed_time_ms");


            var data = new DBTableData<int, SqlConnection>(sd)
            {
                ConnectionString = "Server=.;Database=DatePtAndrei;Trusted_Connection=True;",
                FieldNameToMark = "id",
                TableName = "active_slowquery",
                //Fields=new string[] { "id", "[session_id]" }
            };

            IReceive r = new ReceiverTableSQLServerInt(data);
            
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, r);
            job.Senders.Add(0, csvExport);
            job.Senders.Add(1, xmlExport);
            job.Senders.Add(2, jsonExport);
            //job.Senders.Add(3, influx);

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting=Formatting.Indented,
                Error = HandleDeserializationError
                //ConstructorHandling= ConstructorHandling.AllowNonPublicDefaultConstructor

            };

            //var serialized = JsonConvert.SerializeObject(job, settings);
            //var deserialized = JsonConvert.DeserializeObject(File.ReadAllText("c.txt"), settings);
            

            ////File.WriteAllText("a.txt", serialized);
            //File.WriteAllText("b.txt",JsonConvert.SerializeObject(deserialized as IJob, settings));
            //return; 
            job.Execute().Wait();
            return;
            Process.Start("notepad.exe","a.json");
            //var connection = new SqliteConnection();
            //connection.ConnectionString = "DataSource=:memory:";
            //connection.Open();
            //Console.WriteLine("Hello World!");
            //var options = new DbContextOptionsBuilder<ApiContext>()
            //   //.UseInMemoryDatabase(databaseName: "Add_writes_to_database")
            //   .UseSqlite(connection)
            //   //.UseSqlite("DataSource=:memory:")
            //   .Options;
            //using (var context = new ApiContext(options))
            //{
            //    context.Database.EnsureCreated();
            //}
            //var apiContext = new ApiContext(options);
            //apiContext.Posts.Add(new Post()
            //{
            //    Id = 1,
            //    UserId = 1,
            //    Content = "Post1"
            //});
            //apiContext.SaveChanges();
            //var db = apiContext.Database;
            //var cmd = connection.CreateCommand();
            //cmd.CommandText = "select count(*) from Posts";
            //var i=cmd.ExecuteScalar();

            //var c = ClaimsPrincipal.Current;
            //if (c == null)
            //{

            //}
            //var n = Thread.CurrentThread.Name;
            //Console.WriteLine("ASD");
        }

        private static void brrbrrrr()
        {
            ISerializeData sd = new SerializeDataInMemory();

            var data = new DBTableData<int, SqlConnection>(sd)
            {
                ConnectionString = @"Server=(local)\SQL2016;Database=DatePtAndrei;Trusted_Connection=True;",
                FieldNameToMark = "id",
                TableName = "active_slowquery_test"
            };
            IReceive r = new ReceiverTableSQLServerInt(data);

            ISend esExport = new SenderToElasticSearch(@"http://localhost:9200", "ix004", "type007", "id");

            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, r);
            job.Senders.Add(0, esExport);

            job.Execute().Wait();
        }

        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            var currentError = e.ErrorContext.Error.Message;
            e.ErrorContext.Handled = true;

        }
    }
}