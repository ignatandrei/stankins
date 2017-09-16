using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverCSV;
using ReceiverFileSystem;
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
    public class TestAsyncReceiverMultiple
    {

        [TestMethod]
        public async Task Test2Receivers()
        {

            #region arrange
            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "Test2Receivers");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            string fileNameToWrite = "andrei.txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            if (File.Exists(fullNameFile))
                File.Delete(fullNameFile);

            File.WriteAllText(fullNameFile, "andrei ignat");

            IReceive folder = new ReceiverFolderHierarchical(dir, "andrei.txt");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("model,Track_number");
            sb.AppendLine("Ford,B325ROS");
            sb.AppendLine("Audi,PL654CSM");
            sb.AppendLine("BMW,B325DFH");
            sb.AppendLine("Ford,B325IYS");

            string filenameCSV = "mycsv.csv";
            if (File.Exists(filenameCSV))
                File.Delete(filenameCSV);
            File.WriteAllText(filenameCSV, sb.ToString());
            //System.Diagnostics.Process.Start("notepad.exe", filenameCSV);
            var CSVfile = new ReceiverCSVFileInt(filenameCSV, Encoding.ASCII);            
            #endregion
            #region ACT
            var m = new AsyncReceiverMultiple(CSVfile, folder);
            await m.LoadData();
            #endregion
            #region assert
            //4 records in CSV + 1 folder + 1 file
            Assert.AreEqual(6, m.valuesRead.Length);
            #endregion


        }
    }
}
