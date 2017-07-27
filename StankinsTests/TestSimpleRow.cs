using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nest;
using SenderElasticSearch;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestSimpleRow
    {
        //public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestSimpleRowSameKeysValues()
        {
            //Same keys & values, same order
            #region arange
            SimpleRow r1 = new SimpleRow();
            r1.Values = new Dictionary<string, object>();
            r1.Values["PersonID"] = 1001;
            r1.Values["FirstName"] = "John";
            r1.Values["LastName"] = "Doe";

            SimpleRow r2 = new SimpleRow();
            r2.Values = new Dictionary<string, object>();
            r2.Values["PersonID"] = 1001;
            r2.Values["FirstName"] = "John";
            r2.Values["LastName"] = "Doe";
            #endregion

            #region act
            bool result1 = r1.Equals(r1, r2); 
            bool result2 = r2.Equals(r2, r1); 
            #endregion

            #region assert
            Assert.IsTrue(result1, "First object should have the same Values as second object");
            Assert.IsTrue(result2, "Last object should have the same Values as first object");
            #endregion

            //Same keys & values, diff. order
            #region arange
            r1 = new SimpleRow();
            r1.Values = new Dictionary<string, object>();
            r1.Values["PersonID"] = 1001;
            r1.Values["FirstName"] = "John";
            r1.Values["LastName"] = "Doe";

            r2 = new SimpleRow();
            r2.Values = new Dictionary<string, object>();
            r2.Values["FirstName"] = "John";
            r2.Values["LastName"] = "Doe";
            r2.Values["PersonID"] = 1001;
            #endregion

            #region act
            result1 = r1.Equals(r1, r2);
            result2 = r2.Equals(r2, r1);
            #endregion

            #region assert
            Assert.IsTrue(result1, "First object should have the same Values as second object");
            Assert.IsTrue(result2, "Last object should have the same Values as first object");
            #endregion
        }

        [TestMethod]
        public void TestSimpleRowDiffKeysAndOrValues()
        {
            //Less keys & same values
            #region arange
            SimpleRow r1 = new SimpleRow();
            r1.Values = new Dictionary<string, object>();
            r1.Values["PersonID"] = 1001;
            r1.Values["FirstName"] = "John";
            r1.Values["LastName"] = "Doe";

            SimpleRow r2 = new SimpleRow();
            r2.Values = new Dictionary<string, object>();
            r2.Values["PersonID"] = 1001;
            r2.Values["FirstName"] = "John";
            #endregion

            #region act
            bool result1 = r1.Equals(r1, r2);
            bool result2 = r2.Equals(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion

            //Same keys & diff. values
            #region arange
            r1 = new SimpleRow();
            r1.Values = new Dictionary<string, object>();
            r1.Values["PersonID"] = 1001;
            r1.Values["FirstName"] = "John";
            r1.Values["LastName"] = "Doe";

            r2 = new SimpleRow();
            r2.Values = new Dictionary<string, object>();
            r2.Values["PersonID"] = 1001;
            r2.Values["FirstName"] = "Joahna";
            r2.Values["LastName"] = "Doe";
            #endregion

            #region act
            result1 = r1.Equals(r1, r2);
            result2 = r2.Equals(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion

            //Diff. keys & same values
            #region arange
            r1 = new SimpleRow();
            r1.Values = new Dictionary<string, object>();
            r1.Values["PersonID"] = 1001;
            r1.Values["FirstName"] = "John";
            r1.Values["LastName"] = "Doe";

            r2 = new SimpleRow();
            r2.Values = new Dictionary<string, object>();
            r2.Values["ID"] = 1001;
            r2.Values["FirstName"] = "John";
            r2.Values["LastName"] = "Doe";
            #endregion

            #region act
            result1 = r1.Equals(r1, r2);
            result2 = r2.Equals(r2, r1);
            #endregion

            #region assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            #endregion
        }
    }
}