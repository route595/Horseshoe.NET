using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Email
{
    /// <summary>
    /// Email utility methods primarily for Horseshoe.NET
    /// </summary>
    public static class SmtpUtil
    {
        /// <summary>
        /// Creates an <c>SmtpClient</c> (from connection info or settings) for generating emails
        /// </summary>
        /// <param name="connectionInfo">An optional <c>SmtpConnectionInfo</c> instance</param>
        /// <param name="resultantConnectionInfo"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public static SmtpClient GetSmtpClient(SmtpConnectionInfo connectionInfo = null, Action<SmtpConnectionInfo> resultantConnectionInfo = null)
        {
            connectionInfo = connectionInfo?.Clone() ?? new SmtpConnectionInfo();
            if (!string.IsNullOrEmpty(connectionInfo.Server))
            {
                connectionInfo.Source = "user-supplied-server";
                resultantConnectionInfo?.Invoke(connectionInfo);
                return connectionInfo.GetSmtpClient();
            }
            var client = GetDefaultSmtpClient();
            if (client != null)
            {
                connectionInfo.Source = EmailSettings.DefaultPort.HasValue
                    ? "config-server-and-port"
                    : "config-server";
                connectionInfo.Server = EmailSettings.DefaultSmtpServer;
                connectionInfo.Port = EmailSettings.DefaultPort;
                connectionInfo.Credentials = EmailSettings.DefaultCredentials;
                resultantConnectionInfo?.Invoke(connectionInfo);
                return client;
            }
            throw new ValidationException("No SMTP server info was found");
        }

        internal static SmtpClient GetDefaultSmtpClient()
        {
            if (EmailSettings.DefaultSmtpServer == null) 
                return null;
            
            var smtpClient = new SmtpClient(EmailSettings.DefaultSmtpServer);
            if (EmailSettings.DefaultPort.HasValue) smtpClient.Port = EmailSettings.DefaultPort.Value;
            if (EmailSettings.DefaultEnableSsl) smtpClient.EnableSsl = true;
            if (EmailSettings.DefaultCredentials.HasValue)
            {
                smtpClient.Credentials = EmailSettings.DefaultCredentials.Value.ToNetworkCredential();
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
