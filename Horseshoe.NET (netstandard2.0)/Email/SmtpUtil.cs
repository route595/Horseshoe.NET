using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Email
{
    public static class SmtpUtil
    {
        public static SmtpClient WhichSmtpClient(SmtpConnectionInfo connectionInfo = null, CryptoOptions options = null)
        {
            if (connectionInfo != null)
            {
                return connectionInfo.GetSmtpClient(options: options);
            }
            else
            {
                return GetDefaultSmtpClient(options: options);
            }
        }

        public static SmtpClient GetSmtpClient(SmtpConnectionInfo connectionInfo = null, CryptoOptions options = null)
        {
            return WhichSmtpClient(connectionInfo: connectionInfo, options: options) ?? throw new UtilityException("No SMTP server info was found");
        }

        internal static SmtpClient GetDefaultSmtpClient(CryptoOptions options = null)
        {
            if (EmailSettings.DefaultSmtpServer == null) return null;
            
            var smtpClient = new SmtpClient(EmailSettings.DefaultSmtpServer);
            if (EmailSettings.DefaultPort.HasValue) smtpClient.Port = EmailSettings.DefaultPort.Value;
            if (EmailSettings.DefaultEnableSsl) smtpClient.EnableSsl = true;
            if (EmailSettings.DefaultCredentials.HasValue)
            {
                if (EmailSettings.DefaultCredentials.Value.HasSecurePassword)
                {
                    smtpClient.Credentials = EmailSettings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, EmailSettings.DefaultCredentials.Value.SecurePassword, EmailSettings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, EmailSettings.DefaultCredentials.Value.SecurePassword);
                }
                else if (EmailSettings.DefaultCredentials.Value.IsEncryptedPassword)
                {
                    smtpClient.Credentials = EmailSettings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, Decrypt.SecureString(EmailSettings.DefaultCredentials.Value.Password, options: options), EmailSettings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, Decrypt.SecureString(EmailSettings.DefaultCredentials.Value.Password, options: options));
                }
                else if (EmailSettings.DefaultCredentials.Value.Password != null)
                {
                    smtpClient.Credentials = EmailSettings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, EmailSettings.DefaultCredentials.Value.Password, EmailSettings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, EmailSettings.DefaultCredentials.Value.Password);
                }
                else
                {
                    smtpClient.Credentials = EmailSettings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, null as string, EmailSettings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(EmailSettings.DefaultCredentials.Value.UserName, null as string);
                }
            }
            return smtpClient;
        }

        internal static void Validate
        (
            string subject,
            string body,
            StringValues to,
            string from,
            StringValues? attach
        )
        {
            var validationMessages = new List<string>();

            if (to.Count == 0)
            {
                validationMessages.Add("'to' cannot be null or empty, mail cannot be sent without any recipients");
            }

            if (from == null && EmailSettings.DefaultFrom == null)
            {
                validationMessages.Add("Please supply a 'from' address, you may configure this value (key=\"Horseshoe.NET:Email:From\")");
            }

            if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body) && (attach ?? StringValues.Empty).Count == 0)
            {
                validationMessages.Add("Email may preclude 'subject', 'body' or 'attachments' but not all three");
            }

            if (validationMessages.Any())
            {
                throw new ValidationException { ValidationMessages = validationMessages.ToArray() };
            }
        }
    }
}
