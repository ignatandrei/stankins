using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StanskinsImplementation;
using System.Linq;

namespace StankinsTests
{

    [TestClass]
    public class TestUtils
    {

        [TestMethod]
        public void TestUtilsCompareDictionarySameKeysValues()
        {
            //Same keys & values, same order
            #region arange
            Dictionary<string, object> r1 = new Dictionary<string, object>();
            r1["PersonID"] = 1001;
            r1["FirstName"] = "John";
            r1["LastName"] = "Doe";

            Dictionary<string, object> r2 = new Dictionary<string, object>();
            r2["PersonID"] = 1001;
            r2["FirstName"] = "John";
            r2["LastName"] = "Doe";
            #endregion

            #region act
            bool result1 = Utils.CompareDictionary(r1, r2);
            bool result2 = Utils.CompareDictionary(r2, r1);
            #endregion

            #region assert
            Assert.IsTrue(result1, "Both objects should have the same values");
            Assert.IsTrue(result2, "Both objects should have the same values");
            #endregion

            //Same keys & values, diff. order
            #region arange
            r1 = new Dictionary<string, object>();
            r1["PersonID"] = 1001;
            r1["FirstName"] = "John";
            r1["LastName"] = "Doe";

            r2 = new Dictionary<string, object>();
            r2["FirstName"] = "John";
            r2["LastName"] = "Doe";
            r2["PersonID"] = 1001;
            #endregion

            #region act
            result1 = Utils.CompareDictionary(r1, r2);
            result2 = Utils.CompareDictionary(r2, r1);
            #endregion

            #region assert
            Assert.IsTrue(result1, "Both objects should have the same values");
            Assert.IsTrue(result2, "Both objects should have the same values");
            #endregion
        }

        [TestMethod]
        public void TestUtilsCompareDictionaryDiffKeysAndOrValues()
        {
            //Less keys & same values
            #region arange
            Dictionary<string, object> r1 = new Dictionary<string, object>();
            r1 = new Dictionary<string, object>();
            r1["PersonID"] = 1001;
            r1["FirstName"] = "John";
            r1["LastName"] = "Doe";

            Dictionary<string, object> r2 = new Dictionary<string, object>();
            r2 = new Dictionary<string, object>();
            r2["PersonID"] = 1001;
            r2["FirstName"] = "John";
            #endregion

            #region act
            bool result1 = Utils.CompareDictionary(r1, r2);
            bool result2 = Utils.CompareDictionary(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion

            //Same keys & diff. values
            #region arange
            r1 = new Dictionary<string, object>();
            r1["PersonID"] = 1001;
            r1["FirstName"] = "John";
            r1["LastName"] = "Doe";

            r2 = new Dictionary<string, object>();
            r2["PersonID"] = 1001;
            r2["FirstName"] = "Joahna";
            r2["LastName"] = "Doe";
            #endregion

            #region act
            result1 = Utils.CompareDictionary(r1, r2);
            result2 = Utils.CompareDictionary(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion

            //Diff. keys & same values
            #region arange
            r1 = new Dictionary<string, object>();
            r1["PersonID"] = 1001;
            r1["FirstName"] = "John";
            r1["LastName"] = "Doe";

            r2 = new Dictionary<string, object>();
            r2["ID"] = 1001;
            r2["FirstName"] = "John";
            r2["LastName"] = "Doe";
            #endregion

            #region act
            result1 = Utils.CompareDictionary(r1, r2);
            result2 = Utils.CompareDictionary(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion
        }

        [TestMethod]
        public void TestUtilsToDictionaryStringObject()
        {
            #region arange
            List<Dictionary<string, string>> source = new List<Dictionary<string, string>>();
            source.Add( new Dictionary<string, string>() { {"PersonID", "1001"}, {"FirstName", "John"}, {"LastName", "Doe"} } );
            source.Add(new Dictionary<string, string>() { { "PersonID", "1002" }, { "FirstName", "Joanna" }, { "LastName", "Doe" } });
            #endregion

            #region act
            var result = source.ToDictionaryStringObject();
            #endregion

            #region assert
            //Same count ?
            Assert.AreEqual(2, result.Count);
            //Same items ?
            var p0 = result[0];
            var p1 = result[1];
            Assert.AreEqual("1001", (string)p0["PersonID"]);
            Assert.AreEqual("John", (string)p0["FirstName"]);
            Assert.AreEqual("Doe", (string)p0["LastName"]);
            Assert.AreEqual("1002", (string)p1["PersonID"]);
            Assert.AreEqual("Joanna", (string)p1["FirstName"]);
            Assert.AreEqual("Doe", (string)p1["LastName"]);
            #endregion
        }

    }
}
