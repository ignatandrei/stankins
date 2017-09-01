using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverCSV
    {
        [TestMethod]
        public async Task TestFileCSV()
        {
            #region ARRANGE
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("model,Track_number");
            sb.AppendLine("Ford,B325ROS");
            sb.AppendLine("Audi,PL654CSM");
            sb.AppendLine("BMW,B325DFH");
            sb.AppendLine("Ford,B325IYS");
             
            string filename = SimpleJobConditionalTransformersTest.DeleteFileIfExists( "mycsv.csv");

            File.WriteAllText( filename, sb.ToString());

            #endregion
            #region ACT
            var CSVfile = new ReceiverCSVFileInt(filename,Encoding.ASCII);
            await CSVfile.LoadData();

            #endregion
            #region ASSERT
            Assert.AreEqual(4, CSVfile.valuesRead.Length);
            Assert.AreEqual("Ford", CSVfile.valuesRead[0].Values["model"].ToString());
            Assert.AreEqual("B325ROS", CSVfile.valuesRead[0].Values["Track_number"].ToString());
            #endregion

        }
    }
}
