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
        public async Task TestFilterComparableSerialization()
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
    }
}
