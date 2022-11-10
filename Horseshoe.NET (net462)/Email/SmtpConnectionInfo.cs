using System.Net;
using System.Net.Mail;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Email
{
    public class SmtpConnectionInfo
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public Credential? Credentials { get; set; }
        public bool EnableSsl { get; set; }

        internal SmtpClient GetSmtpClient(CryptoOptions options = null)
        {
            if (Server == null) return null;

            var smtpClient = new SmtpClient(Server);

            if (Port.HasValue) smtpClient.Port = Port.Value;

            if (EnableSsl) smtpClient.EnableSsl = true;

            if (Credentials.HasValue)
            {
                smtpClient.Credentials = Credentials.Value.ToNetworkCredential();
            }
            return smtpClient;
        }
    }
}
