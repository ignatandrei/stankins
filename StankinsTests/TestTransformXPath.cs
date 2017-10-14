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
    public class TestTransformXPath
    {
        [TestMethod]
        public async Task TestTransformXPathSimple()
        {
            #region arange
            var rows = new List<IRow>();
            var row0 = new Mock<IRow>();
            row0.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 0,
                    ["XmlEventData"] = "" 
                }
            );
            rows.Add(row0.Object);

            var row1 = new Mock<IRow>();
            row1.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 1,
                    ["XmlEventData"] = "       "
                }
            );
            rows.Add(row1.Object);

            var row2 = new Mock<IRow>();
            row2.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 2,
                    ["XmlEventData"] = null
                }
            );
            rows.Add(row2.Object);

            var row3 = new Mock<IRow>();
            row3.SetupProperty(it => it.Values,
                new Dictionary<string, object>()
                {
                    ["ID"] = 2,
                    ["XmlEventData"] = 
@"<event name=""wait_info"" package=""sqlos"" timestamp=""2017-10-04T21:17:15.426Z"">
    <data name=""wait_type"">
        <value>3</value>
        <text>LCK_M_S</text>
    </data>
        <data name=""duration"">
        <value>7303</value>
    </data>
        <action name=""database_name"" package=""sqlserver"">
    <value>Test01</value>
    </action>
        <action name=""client_app_name"" package=""sqlserver"">
    <value/>
    </action>
</event>"
                }
            );
            rows.Add(row3.Object);
            #endregion

            #region act
            var transform = new TransformXPath(@"wait_type=XmlEventData(event/data[@name = ""wait_type""]/text/text())[1];duration=XmlEventData(event/data[@name = ""duration""]/value/text())[1];db=XmlEventData(event/action[@name = ""db""]/value/text())[1]");
            transform.valuesRead = rows.ToArray();
            await transform.Run();
            #endregion

            #region assert
            int i = 0;
            foreach (var item in transform.valuesTransformed)
            {
                Assert.IsTrue(item.Values.ContainsKey("wait_type"));
                Assert.IsTrue(item.Values.ContainsKey("duration"));
                Assert.IsTrue(item.Values.ContainsKey("db"));
                if(i < 3)
                {
                    Assert.IsNull(item.Values["wait_type"]);
                    Assert.IsNull(item.Values["duration"]);
                    Assert.IsNull(item.Values["db"]);
                }
                else if( i == 3)
                {
                    Assert.AreEqual("LCK_M_S", item.Values["wait_type"]);
                    Assert.AreEqual("7303", item.Values["duration"]);
                    Assert.IsNull(item.Values["db"]);
                }
                i++;
            }
            #endregion
        }
    }
}