using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverJob;
using SenderHTML;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
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
            string dir = AppContext.BaseDirectory;
            var job = SimpleJobConditionalTransformersTest.GetJobCSV();
            var receiver = new ReceiverFromJob(job);
            string file = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(dir,"a.html"));
            string view = SimpleJobConditionalTransformersTest.DeleteFileIfExists(Path.Combine(dir, "view.cshtml"));
            
            var sender = new Sender_HTMLHierarchicalViz(view,file,"Name");
            File.WriteAllText(view, sender.DefaultExport());
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
