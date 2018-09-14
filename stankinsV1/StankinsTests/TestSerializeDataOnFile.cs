using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StanskinsImplementation;
using System.IO;

namespace StankinsTests
{
    [TestClass]
    public class TestSerializeDataOnFile
    {

        [TestMethod]
        public void TestSerializeDataOnFileSingleValue()
        {
            //string
            #region arrange
            string writeStringValue = "Basescu";
            string fileName = "TestSerializeToFileSingleValue.txt";
            SerializeDataOnFile sdf = new SerializeDataOnFile(fileName);
            #endregion

            #region act
            sdf.SetValue("DummyKeyString", writeStringValue);
            #endregion

            #region assert
            string readStringValue = (string)sdf.GetValue("DummyKeyString");
            Assert.AreEqual(writeStringValue, readStringValue);
            #endregion

            //int
            #region arrange
            int writeIntValue = 12345;
            #endregion

            #region act
            sdf.SetValue("DummyKeyInt", writeIntValue);
            #endregion

            #region assert
            int readIntValue = (int)sdf.GetValue("DummyKeyInt");
            Assert.AreEqual(writeIntValue, readIntValue);
            #endregion
        }

        [TestMethod]
        public void TestSerializeDataOnFileDictionary()
        {
            //string
            #region arrange
            Dictionary<string, object> write = new Dictionary<string, object>();
            write["PersonID"] = 1001;
            write["FirstName"] = "John";
            write["LastName"] = "Doe";

            string fileName = "TestSerializeToFileDictionary.txt";
            SerializeDataOnFile sdf = new SerializeDataOnFile(fileName);
            #endregion

            #region act
            sdf.SetDictionary(write);
            #endregion

            #region assert
            Dictionary<string, object> read = sdf.GetDictionary();
            Assert.IsTrue(Utils.CompareDictionary(write, read));
            #endregion
        }
    }
}
