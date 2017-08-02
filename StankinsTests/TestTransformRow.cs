using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transformers;
using Transformers.BasicTransformersType;

namespace StankinsTests
{
    [TestClass]
    public class TestTransformRow
    {
        [TestMethod]
        public async Task TestTransform2RowToOne()
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
            var addField= "var s=new StanskinsImplementation.RowRead();" +
                "s.Values.Add(\"FullName\",Values[\"LastName\"]?.ToString() +Values[\"FirstName\"]?.ToString());" +
                "s";
            var transform = new TransformRow(addField);
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion
            #region assert
            for (int i = 0; i < transform.valuesTransformed.Length; i++)
            {
                Assert.AreEqual(transform.valuesTransformed[i].Values["FullName"].ToString(),
                    transform.valuesRead[i].Values["LastName"].ToString() + transform.valuesRead[i].Values["FirstName"].ToString());
            }
           

            #endregion
        }
    }
}
