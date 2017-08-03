using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReceiverBookmarkExportChrome;
using SenderCSV;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestSerializeJob
    {
        [TestMethod]
        public async Task TestSerializeReceiveBKChromeTransformOneValueSendCSV()
        {
            var dir = AppContext.BaseDirectory;
            #region arange
            string filename = Path.Combine(dir, "a.csv");
            if (File.Exists(filename))
                File.Delete(filename);

            var receiver = new ReceiverBookmarkFileChrome(@"C:\Users\admin\Documents\bookmarks_7_25_17.html");

            string transformExpression =
                "var secs = double.Parse((oldValue??null).ToString());" +
                "var val=new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(secs);" +
                "val";


            var transform = new TransformOneValueGeneral(transformExpression, "ADD_DATE", "realDate");

            var sender = new SenderToCSV(filename);
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, receiver);
            job.FiltersAndTransformers.Add(0, transform);
            job.Senders.Add(0,sender);

            #endregion
            #region act
            var newJob = new SimpleJob();
            newJob.UnSerialize(job.SerializeMe());
            await newJob.Execute();
            #endregion
            #region assert
            Assert.AreEqual(job.Senders.Count, newJob.Senders.Count);
            Assert.AreEqual(job.Receivers.Count, newJob.Receivers.Count);
            Assert.AreEqual(job.FiltersAndTransformers.Count, newJob.FiltersAndTransformers.Count);
            Assert.IsTrue(File.Exists(filename), $"file {filename} must exists in export csv");
            Assert.AreEqual(5677, File.ReadAllLines(filename).Length);

            #endregion
        }
    }
}
