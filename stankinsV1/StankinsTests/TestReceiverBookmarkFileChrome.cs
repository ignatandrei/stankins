using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverBookmarkExportChrome;
using ReceiverCSV;
using ReceiverHTML;
using Shouldly;
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
            var receiver = new ReceiverBookmarkFileChrome(@"bookmarks_7_25_17.html");
            #endregion
            #region ACT
            
            await receiver.LoadData();

            #endregion
            #region ASSERT
            Assert.AreEqual(5676, receiver.valuesRead.Length);

            #endregion

        }


        [TestMethod]
        public async Task Test_ReceiverHTMLXPath()
        {
            #region ARRANGE
            var receiver = new ReceiverHTMLXPath(@"bookmarks_7_25_17.html",Encoding.UTF8);
            receiver.XPaths = new string[2];
            receiver.XPaths[0] = "//a";
            receiver.XPaths[1] = "//a";
            receiver.AttributeNames = new string[2];
            receiver.AttributeNames[0] = "HREF";
            //second attribute is missing -take inner text
            #endregion
            #region ACT

            await receiver.LoadData();

            #endregion
            #region ASSERT
            Assert.AreEqual(5676, receiver.valuesRead.Length);
            var rr = receiver.valuesRead[0];
            rr.Values.ShouldContainKey("HREF");
            rr.Values.ShouldContainKey("Value1");
            #endregion

        }
    }
}
