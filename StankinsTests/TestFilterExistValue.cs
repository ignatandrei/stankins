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
    public class TestFilterAddRemoveItemsForValue
    {
        public IRow[] GenerateData()
        {
            var rows = new List<IRow>();
            int nrRows = 11;
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
        public async Task TestFilterRemainItemsForValue()
        {
            var data = GenerateData();
            var f = new FilterRemainItemsForValue("FirstName", "1", FilterType.Contains);
            f.valuesRead = data;
            await f.Run();
            //remain 1 and 10
            f.valuesTransformed?.Length.ShouldBe(2);


        }
        [TestMethod]
        public async Task TestFilterRemoveItemsForValue()
        {
            var data = GenerateData();
            var f = new FilterRemoveItemsForValue("FirstName", "1", FilterType.Contains);
            f.valuesRead = data;
            await f.Run();
            //remove 1 and 10
            f.valuesTransformed?.Length.ShouldBe(9);


        }
    }
}