namespace Horseshoe.NET.Email.Sms
{
    public static class TextMessage
    {
        public static event TextMessageSent TextMessageSent;

        public static void SendViaEmail
        (
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            SendViaEmail(null, message, mobileNumber: mobileNumber, carrier: carrier, from: from, connectionInfo: connectionInfo);
        }

        public static void SendViaEmail
        (
            string subject,
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null
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
            TextMessageSent?.Invoke(recipientAddress, message);
        }
    }
}
