using System.Net.Mail;

namespace Horseshoe.NET.Email
{
    /// <summary>
    /// Connection information for building an <c>SmtpClient</c>
    /// </summary>
    public class SmtpConnectionInfo
    {
        /// <summary>
        /// The SMTP server name or DNS alias
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// An optional TCP port, not required if server is listening on the standard port for SMTP
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// SMTP server credentials
        /// </summary>
        public Credential? Credentials { get; set; }

        /// <summary>
        /// Use <c>true</c> to enable SSL on the SMTP client connection
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// The connection info source, for debugging
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Builds an SMPT client from the connection info.
        /// </summary>
        /// <returns></returns>
        public SmtpClient GetSmtpClient()
        {
            if (Server == null) 
                return null;

            var smtpClient = new SmtpClient(Server);

            if (Port.HasValue) 
                smtpClient.Port = Port.Value;

            if (EnableSsl) 
                smtpClient.EnableSsl = true;

            if (Credentials.HasValue)
            {
                smtpClient.Credentials = Credentials.Value.ToNetworkCredential();
            }
            return smtpClient;
        }

        /// <inheritdoc cref="object.MemberwiseClone"/>
        public SmtpConnectionInfo Clone()
        {
            return MemberwiseClone() as SmtpConnectionInfo;
        }
    }
}
