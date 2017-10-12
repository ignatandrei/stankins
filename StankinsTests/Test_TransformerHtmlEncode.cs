using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TransformerHtmlUrl;

namespace StankinsTests
{
    [TestClass]
    public class Test_TransformerHtmlDecode
    {
        [TestMethod]
        public async Task TransformerHtmlDecodeSimpleText()
        {
#region arrange
            var str = "&lt;block type=&#x27;SimpleJob&#x27;&gt;&lt;/block&gt;";
            var rows = new List<IRow>();
            var rowAndrei = new Mock<IRow>();

            rowAndrei.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    
                    ["str"] = str                    
                }
            );

            rows.Add(rowAndrei.Object);
            #endregion
            #region act
            var t = new TransformerHtmlDecode();
            t.valuesRead = rows.ToArray();
            await t.Run();
            #endregion
            #region assert
            t.valuesTransformed.ShouldNotBeNull();
            t.valuesTransformed.Length.ShouldBe(1);
            var val = t.valuesTransformed[0].Values;
            val.ShouldNotBeNull();
            val.ShouldContainKeyAndValue("str", "<block type='SimpleJob'></block>");


            
            #endregion
        }
    }
}
