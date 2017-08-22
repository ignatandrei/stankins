using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestFilterComparableSerializationDeserialization
    {
        [TestMethod]
        public async Task TestFilterComparableEqualSerialization()
        {
            #region arrange
            var filterAsObject = new FilterComparableEqual(typeof(System.Int32), 3000, "ColumnA");
            #endregion

            #region act
            var filterAsString = (string)((TypeConverter)new FilterComparable()).ConvertTo(filterAsObject, typeof(string));
            #endregion

            #region assert
            Assert.IsNotNull(filterAsString);
            Assert.AreEqual("System.Int32 ColumnA = 3000", filterAsString);
            #endregion
        }

        [TestMethod]
        public async Task TestFilterComparableGreaterOrEqualSerialization()
        {
            #region arrange
            var filterAsObject = new FilterComparableGreaterOrEqual(typeof(System.Int32), 3000, "ColumnA");
            #endregion

            #region act
            var filterAsString = (string)((TypeConverter)new FilterComparable()).ConvertTo(filterAsObject, typeof(string));
            #endregion

            #region assert
            Assert.IsNotNull(filterAsString);
            Assert.AreEqual("System.Int32 ColumnA >= 3000", filterAsString);
            #endregion
        }

        [TestMethod]
        public async Task TestFilterComparableEqualDeserialization()
        {
            #region arrange
            string filterAsString = "System.Int32 ColumnA = 3000";
            #endregion

            #region act
            var filterAsObject = (FilterComparableEqual)((TypeConverter)new FilterComparable()).ConvertFrom(filterAsString);
            #endregion

            #region assert
            Assert.IsNotNull(filterAsObject);
            Assert.AreEqual("System.Int32", filterAsObject.ComparableType.ToString());
            Assert.AreEqual("ColumnA", filterAsObject.FieldName);
            Assert.AreEqual(CompareValues.Equal, filterAsObject.HowToCompareValues);
            Assert.AreEqual(3000, filterAsObject.Value);
            #endregion
        }

        [TestMethod]
        public async Task TestFilterComparableGreaterOrEqualDeserialization()
        {
            #region arrange
            string filterAsString = "System.Int32 ColumnA >= 3000";
            #endregion

            #region act
            var filterAsObject = (FilterComparableGreaterOrEqual)((TypeConverter)new FilterComparable()).ConvertFrom(filterAsString);
            #endregion

            #region assert
            Assert.IsNotNull(filterAsObject);
            Assert.AreEqual("System.Int32", filterAsObject.ComparableType.ToString());
            Assert.AreEqual("ColumnA", filterAsObject.FieldName);
            Assert.AreEqual(CompareValues.GreaterOrEqual, filterAsObject.HowToCompareValues);
            Assert.AreEqual(3000, filterAsObject.Value);
            #endregion
        }
    }
}
