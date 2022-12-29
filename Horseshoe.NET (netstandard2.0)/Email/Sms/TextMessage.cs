using System;

namespace Horseshoe.NET.Email.Sms
{
    /// <summary>
    /// Utility methods for generating email-based text messages
    /// </summary>
    public static class TextMessage
    {
        /// <summary>
        /// Sends an email to be converted by the carrier into a text message
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="mobileNumber">The mobile number to which the eventual text message should be sent</param>
        /// <param name="carrier">The carrier</param>
        /// <param name="from">The sender's email address</param>
        /// <param name="connectionInfo">SMTP connection info</param>
        /// <param name="textSent">An action to perform when the text has been sent, includes recipient email address and message text</param>
        /// <exception cref="ValidationException">if any critical part of the email is missing</exception>
        public static void SendViaEmail
        (
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null,
            Action<string, string> textSent = null
        )
        {
            SendViaEmail(null, message, mobileNumber: mobileNumber, carrier: carrier, from: from, connectionInfo: connectionInfo, textSent: textSent);
        }

        /// <summary>
        /// Sends an email to be converted by the carrier into a text message
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message">The message text</param>
        /// <param name="mobileNumber">The mobile number to which the eventual text message should be sent</param>
        /// <param name="carrier">The carrier</param>
        /// <param name="from">The sender's email address</param>
        /// <param name="connectionInfo">SMTP connection info</param>
        /// <param name="textSent"></param>
        /// <exception cref="ValidationException">if any critical part of the email is missing</exception>
        public static void SendViaEmail
        (
            string subject,
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null,
            Action<string, string> textSent = null
        )
        {
            if (mobileNumber == null) throw new ValidationException("mobileNumber cannot be null");
            if (!carrier.HasValue) throw new ValidationException("carrier cannot be null");
            mobileNumber = SmsUtil.ValidateMobileNumber(mobileNumber);

            var recipientAddress = SmsUtil.BuildTextRecipientAddress(mobileNumber, carrier.Value);

            PlainEmail.Send
            (
                subject,
                message,
                to: recipientAddress,
                from: from ?? SmsSettings.DefaultFrom,
                connectionInfo: connectionInfo
            );
            textSent?.Invoke(recipientAddress, message);
        }
    }
}
