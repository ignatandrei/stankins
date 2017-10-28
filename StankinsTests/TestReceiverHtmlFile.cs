using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiverBookmarkExportChrome;
using ReceiverFile;
using Shouldly;
using System.Text;
using System.Threading.Tasks;

namespace StankinsTests
{
    [TestClass]
    public class TestReceiverHtmlFile
    {
        

        [TestMethod]
        public async Task Test_ReceiverHtmlTable()
        {
            #region ARRANGE
            var receiver = new ReceiverHTMLTable(@"HtmlTables\blockly.html", Encoding.UTF8);
            #endregion
            #region ACT

            await receiver.LoadData();

            #endregion
            #region ASSERT
            receiver.valuesRead.ShouldNotBeNull();
            receiver.valuesRead.Length.ShouldBe(131);
            //TODO: add column names and others
            #endregion

        }

        [TestMethod]
        public async Task Test_ReceiverHtmlTableExample1()
        {
            #region ARRANGE
            var receiver = new ReceiverHTMLTable(@"HtmlTables\TableExample1.html", Encoding.UTF8);
            #endregion
            #region ACT

            await receiver.LoadData();

            #endregion
            #region ASSERT
            receiver.valuesRead.ShouldNotBeNull();
            receiver.valuesRead.Length.ShouldBe(1);
            //TODO: add column names and others
            #endregion

        }

        [TestMethod]
        public async Task Test_ReceiverHtmlTableExample2()
        {
            #region ARRANGE
            var receiver = new ReceiverHTMLTable(@"HtmlTables\TableExample2.html", Encoding.UTF8);
            #endregion
            #region ACT

            await receiver.LoadData();

            #endregion
            #region ASSERT
            receiver.valuesRead.ShouldNotBeNull();
            receiver.valuesRead.Length.ShouldBe(1);
            //TODO: add column names and others
            #endregion

        }
    }
}
