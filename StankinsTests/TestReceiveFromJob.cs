using MediaTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverJob;
using SenderHTML;
using SenderToFile;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiveFromJob
    {
        [TestMethod]
        public async Task TestReceiveFromJobConditional()
        {
            #region arrange
            
            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "TestReceiveFromJobConditional");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            var job = SimpleJobConditionalTransformersTest.GetJobCSV();
            var receiver = new ReceiverFromJob(job);
            string file = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(dir,"job.html"));
            //var sender = new Sender_HierarchicalVizJob(file,"Name");
            //var sender = new SenderMediaToFile(file,
            //    new MediaTransformStringToText("<html><body>"),
            //    new MediaTransformDotJob(),
            //    new MediaTransformStringToText("</body></html>")
            //    );
            var export= SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(dir, "export.cshtml"));
            File.WriteAllText(export, Sender_HTMLRazor.DefaultExport());
            var sender = new SyncSenderMultiple(
                new Sender_Text(file, "<html><body>"),
                new Sender_HTMLRazor("TestReceiveFromJobConditional/"+Path.GetFileName(export), file),
                new Sender_HierarchicalVizJob(file, "Name"),
                new Sender_Text(file, "</body></html>")
                );
            var newJob = new SimpleJob();
            newJob.Receivers.Add(0,receiver);
            //newJob.Receivers.Add(1, new ReceiverFromJob(newJob));
            newJob.Senders.Add(0, sender);
            #endregion
            #region act
            await newJob.Execute();
            #endregion
            #region assert
            Assert.IsTrue(File.Exists(file));
            //System.Diagnostics.Process.Start("explorer.exe", file);

#endregion

        }
    }
}
