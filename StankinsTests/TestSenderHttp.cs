using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SenderHttp;
using StankinsInterfaces;
using StanskinsImplementation;
using Moq;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderHttp
    {
        [TestMethod]
        public async Task TestSendHttp()
        {
            //const string UrlGet = "https://requestb.in/pli6q3pl";
            const string UrlPost = "http://httpbin.org/post";
            HttpMethod Verb = HttpMethod.Post;
            const string MediaType = "application/json";
            const string AuthenticationType = "none";
            const string Username = "";
            const string Password = "";


            #region arange
            var m = new Mock<IRow>();
            var rows = new List<IRow>();
            int nrRows = 4;

            for (int i = 0; i < nrRows; i++)
            {
                var row = new Mock<IRow>();
                row.SetupProperty
                (
                    obj => obj.Values,
                    new Dictionary<string, object>()
                    {
                        ["PersonID"] = i,
                        ["FirstName"] = "John " + i,
                        ["LastName"] = "Doe " + i
                    }
                );

                rows.Add(row.Object);
            }

            #endregion


            #region act
            try
            {
                ISend sendHttp = new SenderHttp.SenderHttp(UrlPost, Verb, MediaType, AuthenticationType, Username, Password);
                sendHttp.valuesToBeSent = rows.ToArray();
                await sendHttp.Send();
            }
            catch (HttpRequestException ex)
            {
                // HttpException is expected
                Assert.AreEqual(401, Convert.ToInt32(ex.Message));
            }
            catch (Exception)
            {
                // Any other exception should cause the test to fail
                Assert.Fail();
            }
        }
        #endregion
    }
}
