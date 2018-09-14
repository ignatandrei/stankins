using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using StankinsInterfaces;
using SenderSMTP;
using MailKit.Net.Pop3;

[TestClass]
public class TestSenderSMTP
{
    //public TestContext TestContext { get; set; }

    [TestMethod]
    [TestCategory("ExternalProgramsToBeRun")]
    public async Task TestSendEmail()
    {
        string from = "666def2ad8-a3dd31@inbox.mailtrap.io";
        string to = "bogdan@localhost.com";
        string smtpServer = "smtp.mailtrap.io"; //https://mailtrap.io/ User:"bsahlean@gmail.com" Password:"SDJSf54c9c12fOEIRNYfhdsffdhFTBD5b05f43a99KLXCP" without double quotes; Free plan = maximum 50 emails allowed
        int smtpPort = 2525;
        string subject = "SenderSMTP.test" + DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffffff");
        bool requiresAuthentication = true;
        string user = "0b7c108f1bd651";
        string password = "28fe1e0e2be31f";
        string pop3Server = "mailtrap.io";
        int pop3Port = 9950; // 1100 or 9950

        #region arrange
        //Prepare source data: 2 rows {ID, FirstName, LastName}
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

        var sender = new SenderToSMTP(from, to, string.Empty, string.Empty, subject, string.Empty, false, smtpServer, smtpPort, false, requiresAuthentication, user, password);
        sender.valuesToBeSent = rows.ToArray();
        #endregion

        #region act
        //Send message
        await sender.Send();
        #endregion

        #region assert
        //Read message and check Body (plain text)
        bool emailFound = false;
        using (var client = new Pop3Client())
        {
            client.Connect(pop3Server, pop3Port, false);
            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(user, password);

            for (int i = 0; i < client.Count; i++)
            {
                var message = client.GetMessage(i);
                if (message.Subject == subject)
                {
                    emailFound = true;

                    string[] csvRows = message.TextBody.Split('\n');
                    string[] csvValues0 = csvRows[0].Split(',');
                    string[] csvValues1 = csvRows[1].Split(',');
                    string[] csvValues2 = csvRows[2].Split(',');

                    Assert.AreEqual("PersonID", csvValues0[0]);
                    Assert.AreEqual("FirstName", csvValues0[1]);
                    Assert.AreEqual("LastName", csvValues0[2]);
                    Assert.AreEqual("0", csvValues1[0]);
                    Assert.AreEqual("John 0", csvValues1[1]);
                    Assert.AreEqual("Doe 0", csvValues1[2]);
                    Assert.AreEqual("1", csvValues2[0]);
                    Assert.AreEqual("John 1", csvValues2[1]);
                    Assert.AreEqual("Doe 1", csvValues2[2]);
                    Assert.AreEqual("", csvRows[3]);

                    client.DeleteMessage(i);
                }
            }
            client.Disconnect(true);
        }
        Assert.IsTrue(emailFound);
        #endregion

    }
}
