using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverFolder
    {
        [TestMethod]
        public async Task TestCurrentDir()
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
            IReceive r = new ReceiverFolder(dir,"*.txt");
            await r.LoadData();
            #endregion
            #region assert
            Assert.IsTrue(r.valuesRead?.Length > 0, "must have a file");
            var rowDir = r.valuesRead.FirstOrDefault(it => it.Values["FullName"]?.ToString() == dir);
            Assert.IsNotNull(rowDir);
            var rowFile= r.valuesRead.FirstOrDefault(it => it.Values["FullName"]?.ToString() == fullNameFile);
            Assert.IsNotNull(rowFile);
            var rh = rowFile as IRowReceiveHierarchical;
            Assert.IsNotNull(rh);
            Assert.AreEqual(rh.Parent, rowDir);
            #endregion

        }
    }
}
