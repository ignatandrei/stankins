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
    public class TestTransformerBasic
    {
        
        
        [TestMethod]
        public async Task TestTransformInt2String()
        {
            var dir = AppContext.BaseDirectory;
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
            
            var transform = new TransformerFieldIntString("ID", "NewStringID");
            //var transform = new TransformOneValueGeneral("(oldValue??0).ToString()", "ID", "NewStringID");
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion
            #region assert
            foreach(var item in transform.valuesTransformed)
            {
                Assert.AreEqual(item.Values["ID"].ToString(), item.Values["NewStringID"].ToString());
                Assert.AreEqual(item.Values["ID"].GetType(), typeof(int));
                Assert.AreEqual(item.Values["NewStringID"].GetType(), typeof(string));
            }

            #endregion
        }

        [TestMethod]
        public async Task TestTransformString2Int()
        {
            var dir = AppContext.BaseDirectory;
            #region arange

            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i.ToString(),
                        ["FirstName"] = "Andrei" + i,
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }


            #endregion
            #region act
            var transform = new TransformerFieldStringInt("ID", "NewIntID");
            //var transform = new TransformOneValueGeneral("int.Parse((oldValue??0).ToString())", "ID", "NewIntID");
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion
            #region assert
            foreach (var item in transform.valuesTransformed)
            {
                Assert.AreEqual(item.Values["ID"].ToString(), item.Values["NewIntID"].ToString());
                Assert.AreEqual(item.Values["ID"].GetType(), typeof(string));
                Assert.AreEqual(item.Values["NewIntID"].GetType(), typeof(int));
            }

            #endregion
        }
    }
}
