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
    public class TestTransformRegex
    {

        [TestMethod]
        public async Task TestTransformerRegex()
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
                        ["Data"] = $"Date: 1970/04/{i+10} 18:20:52.309: Log file created.",
                        ["LastName"] = "Ignat" + i
                    }
                );
                if (i % 2 == 0)
                    rowAndrei.Object.Values.Remove("Data");

                rows.Add(rowAndrei.Object);
            }

            #endregion
            #region act
            var tr = new TransformRowRegex( @"^Date:\ (?<date>.{23}).*?$", "Data");
            tr.valuesRead = rows.ToArray();
            await tr.Run();
            #endregion
            #region assert
            tr.valuesTransformed.Length.ShouldBe(nrRows, $"should have {nrRows} values");
            var nr=0;
            foreach(var item in tr.valuesTransformed)
            {
                if (item.Values.ContainsKey("date"))
                    nr++;
            }
            nr.ShouldBe((nrRows-1)/2);
            #endregion
        }


    }
}
