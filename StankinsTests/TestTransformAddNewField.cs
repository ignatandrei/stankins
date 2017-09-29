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
    public class TestTransformAddNewField
    {
        [TestMethod]
        public async Task TestTransformAddNewFieldSimple()
        {
            #region arange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();

                row.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "John" + i
                    }
                );

                rows.Add(row.Object);
            }
            #endregion

            #region act
            var transform = new TransformAddNewField("LastName");
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion

            #region assert
            foreach (var item in transform.valuesTransformed)
            {
                Assert.IsTrue(item.Values.ContainsKey("LastName"));
            }

            #endregion
        }
    }
}