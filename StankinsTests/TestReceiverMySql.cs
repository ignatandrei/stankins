using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using ReceiverDB;
using ReceiverDBMySQL;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverMySql
    {
        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        [TestCategory("RequiresMySQL")]
        public async Task MySqlTestLoadDataWithLatestValueInt()
        {
            #region Arrange

            //ADD A DATABASE AND PUT nrPostsToAdd data into table

            string sqlcommand;
            string testName1 = "TEST";
            bool equality = true;
            RowRead valLoop = null;
            ISerializeData sd = new SerializeDataInMemory();
            IRowReceive[] valuesRead1 = new IRowReceive[100];
            var values = new List<IRowReceive>();
            var ConnString = @"server=localhost;Database=new_schema ;user id=george95; Pwd=1234;";
            Dictionary<int, string> FieldNameToMarks1 = new Dictionary<int, string>();
            DBTableData<int, MySqlConnection> tab2 = new DBTableData<int, MySqlConnection>(sd)
            {
                ConnectionString = ConnString,
                FieldNameToMark = "id",
                TableName = "employees_test;"
            };
            var mysql_obj = new ReceiverTableMySQLInt(tab2);

            using (MySqlConnection connection = new MySqlConnection(ConnString))
            {
                sqlcommand = "CREATE TABLE Employees_test(ID INT(6), Name VARCHAR(50));";
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(sqlcommand, connection))
                {
                    command.ExecuteNonQuery();
                }
                for (int Emp_ID = 1; Emp_ID < 10; Emp_ID++)
                {
                    sqlcommand = "INSERT INTO Employees_test (ID, Name) VALUES (" + Emp_ID + ", '" + testName1 + "');";
                    using (MySqlCommand command = new MySqlCommand(sqlcommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                #endregion
            #region Act
                sqlcommand = "SELECT * FROM Employees_test;";
                using (MySqlCommand command = new MySqlCommand(sqlcommand, connection))
                using (MySqlDataReader reader = (MySqlDataReader)command.ExecuteReader())
                {
                    var nrFields1 = reader.FieldCount;
                    for (int i = 0; i < nrFields1; i++)
                    {
                        FieldNameToMarks1.Add(i, reader.GetName(i));
                    }
                    while (reader.Read())
                    {
                        valLoop = new RowRead();
                        for (int i = 0; i < nrFields1; i++)
                        {
                            object val;
                            val = reader.GetValue(i);
                            if (val != null && val == DBNull.Value)
                            {
                                val = null;
                            }
                            valLoop.Values.Add(FieldNameToMarks1[i], val);
                        }
                        values.Add(valLoop);
                    }
                }
                connection.Close();
            }

            await mysql_obj.LoadData();

            using (MySqlConnection connection = new MySqlConnection(ConnString))
            {
                sqlcommand = "DROP TABLE Employees_test;";
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(sqlcommand, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            #endregion
            #region Assert

            var MySqlLoadResult = mysql_obj.valuesRead.Select(o => o.Values).ToList();
            var AdoNetResult = values.Select(o => o.Values).ToList();
            //AdoNetResult[7]["Name"] = 999;
            for (int index=0; index<MySqlLoadResult.Count; index++)
            {
                if(!MySqlLoadResult[index].SequenceEqual(AdoNetResult[index]))
                {
                    equality = false;
                }
            }
            Assert.IsTrue(equality);
            
            #endregion
        }
    }
}
