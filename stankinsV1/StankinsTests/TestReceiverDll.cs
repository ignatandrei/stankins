using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverDll;
using SenderHTML;
using Shouldly;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    
    public class TestReceiverDll
    {
        [TestMethod]
        public async Task TestReceiveDllImplementation()
        {
#region arrange
            var path = AppContext.BaseDirectory;
            string fileName = Path.Combine(path, "StanskinsImplementation.dll");
            #endregion
#region act
            var rd = new ReceiverFromDllPlain(fileName);
            rd.LoadInterfaces = true;
            rd.LoadBaseClasses = true;
            await rd.LoadData();
            #endregion
            #region assert
            rd.valuesRead.ShouldNotBeNull();
            rd.valuesRead.Length.ShouldBeGreaterThan(1);

            #endregion


        }
        [TestMethod]
        public async Task TestReceiveDllImplementationRelational()
        {
            #region arrange
            var path = AppContext.BaseDirectory;
            string fileName = Path.Combine(path, "StanskinsImplementation.dll");
            #endregion
            #region act
            var rd = new ReceiverFromDllRelational(fileName);           
            await rd.LoadData();
            #endregion
            #region assert
            rd.valuesRead.ShouldNotBeNull();
            rd.valuesRead.Length.ShouldBe(1);
            var assembly = rd.valuesRead[0] as IRowReceiveRelation;
            assembly.ShouldNotBeNull();
            assembly.Values.ShouldNotBeNull();
            assembly.Values.ShouldContainKey("Name");
            assembly.Values["Name"].ShouldBe("StanskinsImplementation");
            assembly.Relations.ShouldNotBeNull();
            assembly.Relations.ShouldContainKey("Types");
            var types = assembly.Relations["Types"];
            types.Count.ShouldBeGreaterThan(0);
            var ssm = types.FirstOrDefault(it => it.Values["Name"]?.ToString() == "SyncSenderMultiple");
            ssm.ShouldNotBeNull("should contain type SyncSenderMultiple");
            var interfaces = ssm.Relations["Interfaces"];
            var props = ssm.Relations["Properties"];
            interfaces.Count.ShouldBeGreaterThan(0);
            props.Count.ShouldBeGreaterThan(0);
            #endregion


        }

        [TestMethod]
        public async Task TestReceiveSendDllHtml()
        {
            #region arrange
            var path = AppContext.BaseDirectory;

            var dir = AppContext.BaseDirectory;
            dir = Path.Combine(dir, "TestReceiveSendDllHtml");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            var fileRazor = Path.Combine(dir, "relationalGeneric.cshtml");
            File.Copy(@"Views\relationalGeneric.cshtml", fileRazor, true);
            
            string fileNameDll = Path.Combine(path, "StanskinsImplementation.dll");
            string filename = Path.Combine(dir, "senderhtml.html");
            if (File.Exists(filename))
                File.Delete(filename);

            var receiverDll = new ReceiverFromDllRelational(fileNameDll);

            ISend senderHTML = new SyncSenderMultiple(
                new Sender_Text(filename, "<html><body>"),
                new Sender_HTMLRazor("TestReceiveSendDllHtml/" + Path.GetFileName(fileRazor), filename),                
                new Sender_HTMLRelationViz("Name",filename),

                new Sender_Text(filename, "</body></html>")
                )
                ;
            #endregion
            #region act
            var job = new SimpleJob();
            job.Receivers.Add(0,receiverDll);
            job.Senders.Add(0,senderHTML);
            await job.Execute();

            #endregion
            #region assert
            File.Exists(filename).ShouldBeTrue($"file {filename} should exists");
            //Process.Start("explorer.exe", filename);
            #endregion


        }
    }
}
