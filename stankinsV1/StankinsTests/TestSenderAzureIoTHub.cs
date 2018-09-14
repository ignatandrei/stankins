using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using SenderAzureIoTHub;
using System.Threading.Tasks;
using Moq;
using StankinsInterfaces;

namespace StankinsTests
{
    [TestClass]
    public class TestSenderAzureIoTHub
    {
        [TestMethod]
        [TestCategory("ExternalProgramsToBeRun")]
        public async Task TestSenderAzureIoTHubSimple()
        {
            #region arrange
            //Sender
            string iotHubUri = "a";
            string deviceId = "b";
            string deviceKey = "c";
            string dataType = "UnitTest";

            var snd = new SenderToAzureIoTHub(iotHubUri, deviceId, deviceKey, dataType);

            //Data to be sent
            var m = new Mock<IRow>();
            var rows = new List<IRow>();
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
                        ["FirstName"] = "John " + i,
                        ["LastName"] = "Doe " + i
                    }
                );

                rows.Add(row.Object);
            }
            #endregion

            #region act
            snd.valuesToBeSent = rows.ToArray();
            await snd.Send();
            #endregion

            #region assert
            //No special assert here. If above code doesn't throw an exception then it's fine.
            #endregion
        }
    }
}
