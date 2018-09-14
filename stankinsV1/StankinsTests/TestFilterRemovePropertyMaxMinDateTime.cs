using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestFilterRemovePropertyMaxMin
    {
        [TestMethod]
        public async Task TestFilterRemovePropertyMaxDateTime()
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
                        ["Data"] = new DateTime(1970, 04, i + 10),
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var tr = new FilterRemovePropertyMaxMinDateTime("Data", GroupingFunctions.Max);
            tr.valuesRead = rows.ToArray();
            await tr.Run();
            #endregion
            #region assert
            tr.valuesTransformed.Length.ShouldBe(nrRows-1, $"should have {nrRows-1} values");
            #endregion

        }
    }
}
