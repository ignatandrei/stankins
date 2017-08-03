using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverBookmarkExportChrome;
using ReceiverCSV;
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
    public class SimpleJobTransformBookmarksChrome
    {
        [TestMethod]
        public async Task Test_ReceiverBookmarkFileChrome()
        {
            var dir = AppContext.BaseDirectory;
            #region ARRANGE
            string pathFile = Path.Combine(dir, "bookmarks_7_25_17.html");
            if (!File.Exists(pathFile))
                throw new ArgumentException($"not found {pathFile}");

            var receiver = new ReceiverBookmarkFileChrome(pathFile);
            string transformExpression =
                "var secs = double.Parse((oldValue??null).ToString());" +
                "var val=new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(secs);" +
                "val";


            var transform = new TransformOneValueGeneral(transformExpression, "ADD_DATE", "realDate");
            #endregion
            #region ACT
            ISimpleJob job = new SimpleJob();
            job.Receivers.Add(0, receiver);
            job.FiltersAndTransformers.Add(0, transform);
            await job.Execute();


            

            #endregion
            #region ASSERT
            Assert.AreEqual(5676, transform.valuesRead.Length);
            Assert.AreEqual(5676, transform.valuesTransformed.Length);
            Assert.AreEqual(new DateTime(2015,5,14,19,15,50).ToString(), transform.valuesTransformed[0].Values["realDate"].ToString());
            #endregion

        }
    }
}
