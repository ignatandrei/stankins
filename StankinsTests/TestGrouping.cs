using CommonDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestGrouping
    {

        [TestMethod]
        public async Task TestTransformerGroupRelationalString()
        {
            #region arrange
            var rows = new List<IRow>();
            int nrRows = 10;
            for (int i = 0; i < nrRows; i++)
            {
                var rowAndrei = new Mock<IRow>();

                rowAndrei.SetupProperty(it => it.Values,
                    new Dictionary<string, object>()
                    {
                        ["ID"] = i,
                        ["FirstName"] = "Andrei" +(i%2),
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var group = new TransformerGroupRelationalString("FirstName");
            group.valuesRead = rows.ToArray();
            await group.Run();
            #endregion
            #region assert
            group.valuesTransformed.Length.ShouldBe(2, "should have andrei0 and andrei1");
            var relFirst= group.valuesTransformed.First() as IRowReceiveRelation;
            var relLast= group.valuesTransformed.First() as IRowReceiveRelation;
            relFirst.Relations["childs"].Count.ShouldBe(5);
            relLast.Relations["childs"].Count.ShouldBe(5);

            #endregion
        }


    }
}
