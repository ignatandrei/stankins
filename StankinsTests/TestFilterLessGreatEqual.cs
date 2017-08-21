using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestFilterLessGreatEqual
    {
        public IRow[] GenerateData()
        {
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei" + i,
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }
            return rows.ToArray();
        }
        [TestMethod]
        public async Task TestLess()
        {
            int value = 3;
            #region arrange
            var f = new FilterComparableLess(typeof(int), 3, "ID");
            f.valuesRead = GenerateData();
            #endregion
            #region act
            await f.Run();
            #endregion
            #region assert
            Assert.AreEqual(value, f.valuesTransformed.Length);
            #endregion  

        }
        [TestMethod]
        public async Task TestEqual()
        {
            int value = 3;
            #region arrange
            var f = new FilterComparableEqual(typeof(int), value, "ID");
            f.valuesRead = GenerateData();
            #endregion
            #region act
            await f.Run();
            #endregion
            #region assert
            Assert.AreEqual(1, f.valuesTransformed.Length);
            #endregion  

        }
        [TestMethod]
        public async Task TestGreater()
        {
            int value = 3;
            #region arrange
            var f = new FilterComparableGreat(typeof(int), value, "ID");
            f.valuesRead = GenerateData();
            #endregion
            #region act
            await f.Run();
            #endregion
            #region assert
            //1 because it starts at 0
            Assert.AreEqual(f.valuesRead.Length-value-1, f.valuesTransformed.Length);
            #endregion  

        }
    }
}
