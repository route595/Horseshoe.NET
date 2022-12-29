using System;
using System.Net.Mail;
using System.Text;

using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Email
{
    /// <summary>
    /// Utility methods for generating HTML formatted emails
    /// </summary>
    public static class HtmlEmail
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
        /// <param name="footerHtml">Optional footer</param>
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
            string footerHtml = null,
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
                Body = JoinBodyAndFooter(body ?? "", footerHtml ?? EmailSettings.DefaultFooterText),
                BodyEncoding = encoding ?? Encoding.Default,
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = true
            };

            foreach (var recipient in to)
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

        private static string JoinBodyAndFooter(string body, string footerHtml)
        {
            if (footerHtml == null) 
                return body;
            var oIndex = body.ToLower().IndexOf("<body");
            var cIndex = body.ToLower().IndexOf("</body>");
            if (cIndex > oIndex && oIndex >= 0)
            {
                var sb = new StringBuilder(body);
                sb.Insert(cIndex, "<br /><br />" + footerHtml);
                return sb.ToString();
            }
            else
            {
                return body + "<br /><br />" + footerHtml;
            }
        }
    }
}
