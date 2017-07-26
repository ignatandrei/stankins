using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SenderCSV;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderCSV
    {
        

        //public TestContext TestContext { get; set; }

        
        [TestMethod]
        public async Task TestSendCSVData()
        {
            var dir = AppContext.BaseDirectory;
            #region arange
            string filename =Path.Combine(dir, "a.csv");
            if (File.Exists(filename))
                File.Delete(filename);

            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei"+i,
                        ["LastName"] = "Ignat"+i
                    }
                );

                rows.Add(rowAndrei.Object);
            }


            #endregion
            #region act
            ISend csvExport = new SenderToCSV(filename);
            csvExport.valuesToBeSent = rows.ToArray();
            await csvExport.Send();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(filename),$"file {filename} must exists in export csv");
            var lines = File.ReadAllLines(filename);
            Assert.AreEqual(nrRows+1, lines.Length);
            var lineNames = lines[0].Split(',');
            Assert.AreEqual(lineNames[0], "ID");
            Assert.AreEqual(lineNames[1], "FirstName");
            Assert.AreEqual(lineNames[2], "LastName");
            
            #endregion
        }
    }
}