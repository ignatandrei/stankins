using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestTransformRemainKey
    {

        [TestMethod]
        public async Task TestTransformRowRemainsProperties()
        {
            #region arrange
            var rows = new List<IRow>();
            int nrRows = 7;
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
                if (i % 1 == 0)
                    rowAndrei.Object.Values.Remove("Data");

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var tr = new TransformRowRemainsProperties("ID","LastName");
            tr.valuesRead = rows.ToArray();
            await tr.Run();
            #endregion
            #region assert
            tr.valuesTransformed.Length.ShouldBe(nrRows, $"should have {nrRows} values");
            
            foreach (var item in tr.valuesTransformed)
            {
                item.Values.ShouldContainKey("ID");
                item.Values.ShouldContainKey("LastName");
                item.Values.ShouldNotContainKey("FirstName");
                
            }
            
            #endregion
        }


    }
}
