using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class TestTransformIntoDate
    {

        [TestMethod]
        public async Task TestTransformToStringIntoDate()
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
                        ["Data"] = System.DateTime.Now.ToString(),
                        ["LastName"] = "Ignat" + i
                    }
                );
                
                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var tr = new TransformerFieldStringToDate("Data","NewData");
            tr.valuesRead = rows.ToArray();
            await tr.Run();
            #endregion
            #region assert
            tr.valuesTransformed.Length.ShouldBe(nrRows, $"should have {nrRows} values");
            var nr = 0;
            foreach (var item in tr.valuesTransformed)
            {
                if (item.Values.ContainsKey("NewData"))
                    nr++;
            }
            nr.ShouldBe(nrRows);
            #endregion
        }

        [TestMethod]
        public async Task TestTransformStringIntoDate()
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
                        ["Data"] = $"1970/04/{i + 10} 18:20:52.309",
                        ["LastName"] = "Ignat" + i
                    }
                );

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var tr = new TransformerFieldStringToDate("Data", "NewData","yyyy/MM/dd HH:mm:ss.fff");
            tr.valuesRead = rows.ToArray();
            await tr.Run();
            #endregion
            #region assert
            tr.valuesTransformed.Length.ShouldBe(nrRows, $"should have {nrRows} values");
            var nr = 0;
            foreach (var item in tr.valuesTransformed)
            {
                if (item.Values.ContainsKey("NewData"))
                {
                    var val =DateTime.Parse(item.Values["NewData"].ToString());
                    val.Year.ShouldBe(1970);
                    nr++;
                }
            }
            nr.ShouldBe(nrRows);
            #endregion
        }

    }
}
