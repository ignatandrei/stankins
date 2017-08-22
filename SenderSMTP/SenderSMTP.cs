using System;
using System.IO;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using CsvHelper;
using MediaTransform;

namespace SenderSMTP
{
    /// <summary>
    /// TODO :
    /// make generic and let
    /// implementation be CSV or attachment or...
    /// </summary>
    public class SenderToSMTP : ISend
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public string Cc { get; set; }
        public string Bcc { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public bool IsBodyHtml { get; private set; }

        public string SmtpServer { get; private set; }
        public int SmtpPort { get; private set; }
        public bool EnableSsl { get; private set; }
        public bool RequiresAuthentication { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }

        public IRow[] valuesToBeSent { set; get; }

        public SenderToSMTP()
        {

        }

        public SenderToSMTP(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml, string smtpServer, int smtpPort = 25, bool enableSsl = false, bool requiresAuthentication = true, string user = "", string password = "")
        {
            this.From = from;
            this.To = to;
            this.Cc = cc;
            this.Bcc = bcc;
            this.Subject = subject;
            this.Body = body;
            this.IsBodyHtml = IsBodyHtml;
            this.SmtpServer = smtpServer;
            this.SmtpPort = smtpPort;
            this.EnableSsl = enableSsl;
            this.RequiresAuthentication = requiresAuthentication;
            this.User = user;
            this.Password = password;
        }

        private void Validation()
        {
            if (!this.RequiresAuthentication && (!string.IsNullOrEmpty(this.User) || !string.IsNullOrEmpty(this.Password)))
            {
                throw new Exception("If RequiresAuthentication is false then user and password should be empty");
            }
            if (this.RequiresAuthentication && (string.IsNullOrEmpty(this.User) || string.IsNullOrEmpty(this.Password)))
            {
                //throw new Exception("If RequiresAuthentication is true then user and password are mandatory");
                throw new NotImplementedException();
            }
        }

        public async Task Send()
        {
            //Initialization
            Validation();

            //Generate email body
            var mediaCSV = new MediaTransformCSV();
            mediaCSV.valuesToBeSent = valuesToBeSent;
            await mediaCSV.Run();
            //TextWriter writer = new StringWriter();
            //using (var csv = new CsvWriter(writer))
            //{
            //    foreach (var row in valuesToBeSent)
            //    {
            //        if (!hasHeader)
            //        {
            //            foreach(var item in row.Values.Keys)
            //            {
            //                csv.WriteField<string>(item);
            //            }
            //            csv.NextRecord();
            //            hasHeader = true;
            //        }

            //        foreach (var item in row.Values.Values)
            //        {
            //            csv.WriteField<object>(item);
            //        }
            //        csv.NextRecord();
            //    }
            //}

            this.Body = this.Body + mediaCSV.Result;

            //Send email
            //Using MailKit 
            //https://www.stevejgordon.co.uk/how-to-send-emails-in-asp-net-core-1-0
            //https://www.joeaudette.com/blog/2016/05/08/sending-smtp-email-on-aspnet-core-with-mailkit
            //TODO: In .Net Core 2.0 replace MailKit with MailMessage
            var message = new MimeMessage();

            foreach (var emailAddress in this.From.Split(';'))
            {
                if (string.IsNullOrEmpty(emailAddress))
                {
                    break;
                }
                message.From.Add(new MailboxAddress(emailAddress));
            }
            foreach (var emailAddress in this.To.Split(';'))
            {
                if (string.IsNullOrEmpty(emailAddress))
                {
                    break;
                }
                message.To.Add(new MailboxAddress(emailAddress));
            }
            foreach (var emailAddress in this.Cc.Split(';'))
            {
                if(string.IsNullOrEmpty(emailAddress))
                {
                    break;
                }
                message.Cc.Add(new MailboxAddress(emailAddress));
            }
            foreach (var emailAddress in this.Bcc.Split(';'))
            {
                if (string.IsNullOrEmpty(emailAddress))
                {
                    break;
                }
                message.Bcc.Add(new MailboxAddress(emailAddress));
            }
            message.Subject = this.Subject;
            message.Body = new TextPart((this.IsBodyHtml ? "html" : "plain")) { Text = this.Body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(this.SmtpServer, this.SmtpPort, (this.EnableSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None) ).ConfigureAwait(false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                if (this.RequiresAuthentication)
                {
                    await client.AuthenticateAsync(this.User,this.Password).ConfigureAwait(false);
                }
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
