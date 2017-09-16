using MediaTransform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverFileSystem;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestMediaDot
    {
        [TestMethod]
        public async Task TestSampleDataMediaDot()
        {
            #region arrange
            var dir = AppContext.BaseDirectory;

            foreach (var item in Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories))
            {
                File.Delete(item);
            }

            string fileNameToWrite = "andrei.txt";
            string fullNameFile = Path.Combine(dir, fileNameToWrite);
            File.WriteAllText(fullNameFile, "andrei ignat");
            #endregion  
            #region arrange
            IReceive r = new ReceiverFolderHierarchical(dir, "*.txt");
            await r.LoadData();
            var m = new MediaTransformDotFolderAndFiles();
            m.valuesToBeSent = r.valuesRead;
            await m.Run();
            #endregion
            #region assert
            Assert.IsTrue(m.Result.Contains("[label=\"andrei.txt"),"result should contain " +m.Result);
            Assert.IsTrue(m.Result.Contains(" -> "));
            #endregion

        }
    }
}
