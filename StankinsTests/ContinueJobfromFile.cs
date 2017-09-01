using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverCSV;
using ReceiverFile;
using SenderToFile;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class ContinueJobfromFile
    {
        [TestMethod]
        public async Task TransmitDataJob2JobFileHardDisk()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("model,Track_number");
            sb.AppendLine("Ford,B325ROS");
            sb.AppendLine("Audi,PL654CSM");
            sb.AppendLine("BMW,B325DFH");
            sb.AppendLine("Ford,B325IYS");

            string filename = SimpleJobConditionalTransformersTest.DeleteFileIfExists("mycsv.csv");

            File.WriteAllText(filename, sb.ToString());
            var CSVReceiver = new ReceiverCSVFileInt(filename, Encoding.ASCII);
            string bin = SimpleJobConditionalTransformersTest.DeleteFileIfExists("a.bin");
            var senderBinary = new Sender_Binary(bin);

            var job = new SimpleJob();
            job.Receivers.Add(0, CSVReceiver);
            job.Senders.Add(0, senderBinary);

            var continueJob = new SimpleJob();
            var binReceiver = new ReceiverFileFromStorageBinary(bin);
            continueJob.Receivers.Add(0, binReceiver);


            #region ACT
            await job.Execute();
            await continueJob.Execute();
            #endregion
            #region assert
            Assert.AreEqual(4, binReceiver.valuesRead.Length);
            Assert.AreEqual("Ford", binReceiver.valuesRead[0].Values["model"]);
            Assert.AreEqual("B325IYS", binReceiver.valuesRead[3].Values["Track_number"]);
            #endregion

        }
    }
}
