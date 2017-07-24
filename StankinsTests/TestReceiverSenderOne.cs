using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDBSQLite;
using SenderCSV;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverSenderOne
    {

        [TestMethod]
        public async Task TestOneReceiverAndOneSender()
        {
            #region ARRANGE
            var dir = AppContext.BaseDirectory;
            string filename = Path.Combine(dir, "a.csv");
            if (File.Exists(filename))
                File.Delete(filename);
            ISend csvExport = new Sender_CSV(filename);

            //ADD A DATABASE AND PUT nrPostsToAdd data into table
            ISerializeData sd = new SerializeDataInMemory();

            var connection = new SqliteConnection();
            connection.ConnectionString = "DataSource=:memory:";
            connection.Open();

            var options = new DbContextOptionsBuilder<ApiContext>()
             .UseSqlite(connection)
             .Options;
            using (var context = new ApiContext(options))
            {
                context.Database.EnsureCreated();
            }
            int nrPostsToAdd = 10;
            using (var contextAdd = new ApiContext(options))
            {
                for (int i = 0; i < nrPostsToAdd; i++)
                {
                    contextAdd.Posts.Add(new Post()
                    {
                        Id = i + 1,
                        UserId = i * 2,
                        Content = "Post" + i
                    });
                }
                await contextAdd.SaveChangesAsync();
            }
            using (var dt = new DBTableDataSqliteMemory<int>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",
                FieldNameToMark = "Id",
                TableName = "Posts"
            })
            {
                #endregion
                #region act
                IReceive r = new ReceiverTableSQLiteInt(dt);
                IJob job = new SimpleJob();
                job.Receivers.Add(0,r);
                job.Senders.Add(0,csvExport);
                await job.Execute();

                #endregion

                #region assert
                Assert.IsTrue(File.Exists(filename), $"file {filename} must exists in export csv");
                var lines = File.ReadAllLines(filename);
                Assert.AreEqual(nrPostsToAdd+1, lines.Length);
                var lineNames = lines[0].Split(',');
                Assert.AreEqual("Id", lineNames[0]);
                Assert.AreEqual("Content", lineNames[1] );
                Assert.AreEqual("LastModifiedOrCreatedTime", lineNames[2]);
                Assert.AreEqual("UserId",lineNames[3]);

                #endregion

            }



        }
    }
}
