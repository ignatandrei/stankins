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
    public class TestTransformerRemoveColumn
    {
        [TestMethod]
        public async Task TestTransformRowRemoveField()
        {
            #region arange
            
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


            #endregion
            #region act
            var transform = new TransformRowRemoveField("ID");
            transform.valuesRead = rows.ToArray();
            await transform.Run();

            #endregion
            #region assert
            for (int i = 0; i < transform.valuesTransformed.Length; i++)
            {
                var item = transform.valuesTransformed[i];
                Assert.IsFalse(item.Values.ContainsKey("ID"));
                Assert.AreEqual(item.Values["FirstName"], transform.valuesRead[i].Values["FirstName"]);
            }
            
            #endregion
        }

        
    }
}
