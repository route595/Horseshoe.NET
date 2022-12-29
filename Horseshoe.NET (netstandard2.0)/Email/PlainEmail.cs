using System;
using System.Net.Mail;
using System.Text;

using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Email
{
    /// <summary>
    /// Utility methods for generating unformatted emails
    /// </summary>
    public static class PlainEmail
    {
        /// <summary>
        /// Sends a plain email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email content</param>
        /// <param name="to">Recipient email address(es)</param>
        /// <param name="cc">CC: email address(es)</param>
        /// <param name="bcc">BCC: email address(es)</param>
        /// <param name="from">Sender email address</param>
        /// <param name="attach">Optional attachment file path(s)</param>
        /// <param name="footerText">Optional footer text</param>
        /// <param name="encoding">Optional email body encoding</param>
        /// <param name="connectionInfo">SMTP connection info</param>
        /// <exception cref="ValidationException">if any critical part of the email is missing</exception>
        public static void Send
        (
            string subject,
            string body,
            StringValues to,
            StringValues cc = default,
            StringValues bcc = default,
            string from = null,
            StringValues attach = default,
            string footerText = null,
            Encoding encoding = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            // create the mail client
            var smtpClient = SmtpUtil.GetSmtpClient(connectionInfo: connectionInfo);

            // validate and create the mail message
            SmtpUtil.Validate
            (
                body,
                subject,
                to,
                from,
                attach
            );

            var mailMessage = new MailMessage()
            {
                Subject = subject ?? "",
                Body = JoinBodyAndFooter(body ?? "", footerText ?? EmailSettings.DefaultFooterText),
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = false
            };
            if (encoding != null)
            {
                mailMessage.BodyEncoding = encoding;
            }

            foreach (var recipient in to.ToArray())
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            foreach (var recipient in cc)
            {
                mailMessage.CC.Add(new MailAddress(recipient));
            }

            foreach (var recipient in bcc)
            {
                mailMessage.Bcc.Add(new MailAddress(recipient));
            }

            foreach (var attachment in attach)
            {
                mailMessage.Attachments.Add(new Attachment(attachment));
            }

            // send mail
            smtpClient.Send(mailMessage);
        }

        private static string JoinBodyAndFooter(string body, string footerText)
        {
            if (footerText == null) 
                return body;
            return body + Environment.NewLine + Environment.NewLine + footerText;
        }
    }
}
