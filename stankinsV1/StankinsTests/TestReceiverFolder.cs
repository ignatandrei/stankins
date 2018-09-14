using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using Shouldly;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestRelationalHierarchical
    {
        [TestMethod]
        public async Task TestCurrentDirHierarchical()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;

            string filename = Path.Combine(dir, "a.html");
            if (File.Exists(filename))
                File.Delete(filename);


            string fileNameToWrite =  Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            #endregion
            #region act
            IReceive r = new ReceiverFolderHierarchical(dir,"*.txt");
            await r.LoadData();
            #endregion
            #region assert
            Assert.IsTrue(r.valuesRead?.Length > 0, "must have a file");
            var rowDir = r.valuesRead.FirstOrDefault(it => it.Values["FullName"]?.ToString() == dir);
            Assert.IsNotNull(rowDir);
            var rowFile= r.valuesRead.FirstOrDefault(it => it.Values["FullName"]?.ToString() == fullNameFile);
            Assert.IsNotNull(rowFile);
            var rh = rowFile as IRowReceiveHierarchicalParent;
            Assert.IsNotNull(rh);
            Assert.AreEqual(rh.Parent, rowDir);
            #endregion

        }

        [TestMethod]
        public async Task TestCurrentDirRelational()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;

           


            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            #endregion
            #region act
            IReceive r = new ReceiverFolderRelational(dir, "*.txt");
            await r.LoadData();
            #endregion
            #region assert
            r.valuesRead.ShouldNotBeNull();
            r.valuesRead.Length.ShouldBe(1);
            var rowDir = r.valuesRead[0];
            rowDir.ShouldNotBeNull();
            rowDir.Values.ShouldNotBeNull();
            rowDir.Values["FullName"].ShouldBe(dir);
            var rr = rowDir as IRowReceiveRelation;
            rr.ShouldNotBeNull();
            rr.Relations.ShouldContainKey("files");
            var files = rr.Relations["files"];
            files.Count.ShouldBeGreaterThanOrEqualTo(1);
            files.ShouldContain(it => it.Values["Name"].ToString() == fileNameToWrite);
            
            #endregion

        }

        [TestMethod]
        public async Task TestTransformerRelationalToPlain()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;




            string fileNameToWrite = Guid.NewGuid().ToString("N") + ".txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");

            #endregion
            #region act
            IReceive r = new ReceiverFolderRelational(dir, "*.txt");
            await r.LoadData();
            var transform = new TransformerRelationalToPlain();
            transform.valuesRead = r.valuesRead;
            await transform.Run();
            #endregion
            #region assert
            transform.valuesTransformed.ShouldNotBeNull();
            transform.valuesTransformed.Length.ShouldBeGreaterThan(2);
            #endregion

        }
    }
}
