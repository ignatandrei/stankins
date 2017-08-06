using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using StankinsInterfaces;
using SenderSMTP;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using MailKit;
using MailKit.Net.Pop3;

[TestClass]
public class TestSenderSMTP
{
    //public TestContext TestContext { get; set; }

    [TestMethod]
    [TestCategory("ExternalProgramsToBeRun")]
    public async Task TestSendEmail()
    {
        string to = "bogdan@localhost";
        string smtpServer = "127.0.0.1";
        int smtpPort = 25;
        string subject = "SenderSMTP.test" + DateTime.Now.ToString();
        bool requiresAuthentication = false;
        string user = string.Empty;
        string password = string.Empty;

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

        var sender = new SenderToSMTP(to, string.Empty, string.Empty, subject, string.Empty, false, smtpServer, smtpPort, false, requiresAuthentication, user, password);
        sender.valuesToBeSent = rows.ToArray();
        #endregion

        #region act
        await sender.Send(); //smtp4dev should be enabled an should listen on 127.0.0.1:25
        #endregion

        #region assert
        //If it's doesn't throw an exception it's ok
        #endregion

    }
}
