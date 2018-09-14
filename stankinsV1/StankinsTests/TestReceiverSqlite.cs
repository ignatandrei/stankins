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
    public class TestReceiverSqlite
    {
        [TestMethod]
        public async Task TestLoadDataWithLatestValueInt()
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

            #endregion
            #region ACT
            using (var dt = new DBTableDataSqliteMemory<int>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",
                FieldNameToMark = "Id",
                TableName = "Posts"
            })
            {
                IReceive<int> r = new ReceiverTableSQLiteInt(dt);
                await r.LoadData();

                #endregion
                #region ASSERT

                Assert.AreEqual(nrPostsToAdd, r.valuesRead.Length);
                Assert.AreEqual(nrPostsToAdd, r.LastValue);
                #endregion
                #region arange add new data into table
                using (var contextAddNew = new ApiContext(options))
                {

                    contextAddNew.Posts.Add(new Post()
                    {
                        Id = nrPostsToAdd * 2,
                        UserId = nrPostsToAdd,
                        Content = "Post" + nrPostsToAdd
                    });
                    await contextAddNew.SaveChangesAsync();

                }
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(1, r.valuesRead.Length);
                Assert.AreEqual(nrPostsToAdd * 2, r.LastValue);
                #endregion
                #region arange
                //load from the beginning
                r.LastValue = 0;
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(nrPostsToAdd + 1, r.valuesRead.Length);
                Assert.AreEqual(nrPostsToAdd * 2, r.LastValue);
                #endregion
            }
        }
        [TestMethod]
        public async Task TestLoadDataWithLatestValueDateTime()
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

            #endregion
            #region ACT
            using (var dt = new DBTableDataSqliteMemory<DateTime>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",
                FieldNameToMark = "LastModifiedOrCreatedTime",
                TableName = "Posts"
            })
            {
                IReceive<DateTime> r = new ReceiverTableSQLiteDateTime(dt);
                await r.LoadData();

                #endregion
                #region ASSERT

                Assert.AreEqual(nrPostsToAdd, r.valuesRead.Length);
                Assert.AreEqual(0,(int)r.LastValue.Subtract(DateTime.Now).TotalSeconds);
                #endregion
                #region arange add new data into table
                //wait 1 second to add new data
                await Task.Delay(1000);
                using (var contextAddNew = new ApiContext(options))
                {

                    contextAddNew.Posts.Add(new Post()
                    {
                        Id = nrPostsToAdd * 2,
                        UserId = nrPostsToAdd,
                        Content = "Post" + nrPostsToAdd
                    });
                    await contextAddNew.SaveChangesAsync();

                }
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(1, r.valuesRead.Length);
                Assert.AreEqual(0, (int)r.LastValue.Subtract(DateTime.Now).TotalSeconds);
                #endregion
                #region arange
                //load from the beginning
                r.LastValue = DateTime.MinValue;
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(nrPostsToAdd + 1, r.valuesRead.Length);
                Assert.AreEqual(0, (int)r.LastValue.Subtract(DateTime.Now).TotalSeconds);
                #endregion
            }
        }

        [TestMethod]
        public async Task TestLoadWholeTableEachTime()
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

            #endregion
            #region ACT
            using (var dt = new DBTableDataSqliteMemory<FakeComparable>(connection, sd)
            {
                ConnectionString = "DataSource=:memory:",                
                TableName = "Posts"
            })
            {
                IReceive r = new ReceiverWholeTableSQLite(dt);
                await r.LoadData();

                #endregion
                #region ASSERT

                Assert.AreEqual(nrPostsToAdd, r.valuesRead.Length);                
                #endregion
                #region arange add new data into table
                using (var contextAddNew = new ApiContext(options))
                {

                    contextAddNew.Posts.Add(new Post()
                    {
                        Id = nrPostsToAdd * 2,
                        UserId = nrPostsToAdd,
                        Content = "Post" + nrPostsToAdd
                    });
                    await contextAddNew.SaveChangesAsync();

                }
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(nrPostsToAdd+1, r.valuesRead.Length);                
                #endregion
                #region arange
                //load from the beginning
                
                #endregion
                #region act
                await r.LoadData();
                #endregion
                #region assert
                Assert.AreEqual(nrPostsToAdd +1, r.valuesRead.Length);
                
                #endregion
            }
        }
    }
}
