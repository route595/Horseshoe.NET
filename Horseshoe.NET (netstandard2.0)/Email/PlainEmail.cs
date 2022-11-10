using System;
using System.Net.Mail;
using System.Text;

using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Email
{
    public static class PlainEmail
    {
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
            var smtpClient = SmtpUtil.GetSmtpClient(connectionInfo: connectionInfo) ??
                throw new ValidationException("Not enough info to create an SMTP client");

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
                BodyEncoding = encoding ?? Encoding.ASCII,
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = false
            };

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
            if (footerText == null) return body;
            return body + Environment.NewLine + Environment.NewLine + footerText;
        }
    }
}
