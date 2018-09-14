using CommonDB;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SenderBulkCopy;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestRowDataReader
    {

        [TestMethod]
        public async Task TestDataIsOkForRowDataReader()
        {
            #region arrange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei" + i,
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var rr = new RowDataReader(rows.ToArray(), "ID", "FirstName");

            #endregion
            #region assert
            int nrRecords = 0;
            while (await rr.ReadAsync())
            {


                Assert.AreEqual(nrRecords, rr["ID"]);
                Assert.AreEqual("Andrei" + nrRecords, rr["FirstName"]);
                nrRecords++;

            }
            Assert.AreEqual(nrRows, nrRecords);
            #endregion
        }

        
    }
}
