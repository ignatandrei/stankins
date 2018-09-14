using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverCSV;
using ReceiverJSON;
using SenderToFile;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverJSONFile
    {
        [TestMethod]
        public async Task TestFile()
        {
            #region arrange
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("model,Track_number");
            sb.AppendLine("Ford,B325ROS");
            sb.AppendLine("Audi,PL654CSM");
            sb.AppendLine("BMW,B325DFH");
            sb.AppendLine("Ford,B325IYS");

            string filename = SimpleJobConditionalTransformersTest.DeleteFileIfExists("mycsv.csv");

            File.WriteAllText(filename, sb.ToString());
            var CSVfile = new ReceiverCSVFileInt(filename, Encoding.ASCII);
            await CSVfile.LoadData();
            string filenameJSON = SimpleJobConditionalTransformersTest.DeleteFileIfExists("cars.json");

            var sjons = new Sender_JSON(filenameJSON);
            sjons.valuesToBeSent = CSVfile.valuesRead;
            await sjons.Send();


            #endregion
            #region act
            var f = new ReceiverJSONFileInt(filenameJSON, Encoding.UTF8);
            await f.LoadData();
            #endregion
            #region assert            
            
            f?.valuesRead?.Length.ShouldBe(4);
            f.valuesRead[0].Values["model"].ShouldBe("Ford");
            f.valuesRead[0].Values["Track_number"].ShouldBe("B325ROS");
            f.valuesRead[1].Values["model"].ShouldBe("Audi");
            f.valuesRead[1].Values["Track_number"].ShouldBe("PL654CSM");
            f.valuesRead[3].Values["Track_number"].ShouldBe("B325IYS");


            #endregion


        }
    }
}
