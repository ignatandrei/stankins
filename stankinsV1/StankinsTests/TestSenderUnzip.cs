using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using SenderUnzipFiles;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderUnzip
    {
        [TestMethod]
        public async Task TestUnzip()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;
            string existingZip = Path.Combine(dir, "a.zip");
            dir = Path.Combine(dir, "TestUnzip");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            File.Copy(existingZip, Path.Combine(dir, "a.zip"));
            var receive = new ReceiverFolderHierarchical(dir, "*.zip");
            var sender = new SenderUnzipFile(dir);
            #endregion
            #region act
            var job = new SimpleJob();
            job.Receivers.Add(0, receive);
            job.Senders.Add(0, sender);
            await job.Execute();
            #endregion
            #region assert
            string file = Path.Combine(dir, "a.txt");
            Assert.IsTrue(File.Exists(file), $"{file} should be unzipped");

            #endregion
        }
    }
}
