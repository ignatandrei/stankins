using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StanskinsImplementation;
using StankinsInterfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReceiverDBSQLite;
using ReceiverDB;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverSqliteMaxRowsToLoad
    {
        [TestMethod]
        public async Task TestLoadMaxRecordsDataWithLatestValueInt()
        {
            #region ARRANGE
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
                        Id = i+1,
                        UserId = i * 2,
                        Content = "Post" + i
                    });
                }
                await contextAdd.SaveChangesAsync();
            }
            int MaxRecordsToLoad = 3;
            #endregion
            #region ACT
            using (var dt = new DBTableDataSqliteMemory<int>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",
                FieldNameToMark = "Id",
                TableName = "Posts",
                MaxRecordsToRead= MaxRecordsToLoad

            })
            {
                IReceive<int> r = new ReceiverTableSQLiteInt(dt);
                await r.LoadData();

                #endregion
                #region ASSERT

                Assert.AreEqual(MaxRecordsToLoad, r.valuesRead.Length);
                Assert.AreEqual(MaxRecordsToLoad, r.LastValue);
                #endregion
                #region act
                //load next rows
                r = new ReceiverTableSQLiteInt(dt);
                await r.LoadData();

                #endregion
                #region assert
                Assert.AreEqual(MaxRecordsToLoad, r.valuesRead.Length);
                Assert.AreEqual(MaxRecordsToLoad*2, r.LastValue);
                #endregion

            }

        }
        [TestMethod]
        public async Task TestLoadMaxRecordsWithLatestValueDateTime()
        {
            #region ARRANGE
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
            int MaxRecordsToLoad = 3;
            #endregion
            #region ACT
            using (var dt = new DBTableDataSqliteMemory<DateTime>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",
                FieldNameToMark = "LastModifiedOrCreatedTime",
                TableName = "Posts",
                MaxRecordsToRead=MaxRecordsToLoad
            })
            {
                IReceive<DateTime> r = new ReceiverTableSQLiteDateTime(dt);
                await r.LoadData();

                #endregion
                #region ASSERT

                Assert.AreEqual(MaxRecordsToLoad , r.valuesRead.Length);
                Assert.AreEqual(0,(int)r.LastValue.Subtract(DateTime.Now).TotalSeconds);
                #endregion
               
            }
        }
    }
}
