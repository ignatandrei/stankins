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
//using Microsoft.Data.Sqlite;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
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
            ISend csvExport = new Sender_CSV("a.csv");
            ISend xmlExport = new Sender_XML("a.xml");
            ISend jsonExport = new Sender_JSON("a.json");



            var data = new DBTableData<int, SqlConnection>(sd)
            {
                ConnectionString = "Server=.;Database=DatePtAndrei;Trusted_Connection=True;",
                FieldNameToMark = "id",
                TableName = "active_slowquery",
                //Fields=new string[] { "id", "[session_id]" }
            };

            IReceive r = new ReceiverTableSQLServerInt(data);
            
            IJob job = new SimpleJob();
            job.Receivers.Add(0, r);
            job.Senders.Add(0, csvExport);
            job.Senders.Add(1, xmlExport);
            job.Senders.Add(2, jsonExport);
            job.Execute().Wait();
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
    }
}