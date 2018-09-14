using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransformerHtmlUrl;
using Transformers;

namespace StankinsTests
{
    [TestClass]
    public class Test_TransformerKeyValue
    {
        [TestMethod]
        public async Task TransformerKeyValue()
        {
            #region arrange
            var rows = new List<IRow>();
            var m = new Mock<IRow>();
            
            int nrRows = 2;

            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();
                row.SetupProperty
                (
                    obj => obj.Values,
                    new Dictionary<string, object>()
                    {
                        ["PersonID"] = i,
                        ["FirstName"] =(i%2==0)?"": "John " + i,
                        ["LastName"] = (i % 2 == 1) ? "":"Doe " + i
                    }
                );

                rows.Add(row.Object);
            }

            
            #endregion
            #region act
            var t = new TransformKeyValue();
            t.valuesRead = rows.ToArray();
            await t.Run();
            #endregion
            #region assert
            t.valuesTransformed.ShouldNotBeNull();
            t.valuesTransformed.Length.ShouldBe(2);
            var val = t.valuesTransformed[0].Values;
            val.ShouldNotBeNull();
            val.ShouldContainKeyAndValue("key",0);
            val.ShouldContainKeyAndValue("value", "Doe 0");

            val = t.valuesTransformed[1].Values;
            val.ShouldNotBeNull();
            val.ShouldContainKeyAndValue("key", 1);
            val.ShouldContainKeyAndValue("value", "John 1");




            #endregion
        }
    }
}
