using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverBookmarkExportChrome;
using ReceiverCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverBookmarkFileChrome
    {
        [TestMethod]
        public async Task Test_ReceiverBookmarkFileChrome()
        {
            #region ARRANGE
            var receiver = new ReceiverBookmarkFileChrome(@"C:\Users\admin\Documents\bookmarks_7_25_17.html");
            #endregion
            #region ACT
            
            await receiver.LoadData();

            #endregion
            #region ASSERT
            Assert.AreEqual(5676, receiver.valuesRead.Length);
            #endregion

        }
    }
}
