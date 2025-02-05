using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Email
{
    /// <summary>
    /// Configuration settings for Horseshoe.NET.Email
    /// </summary>
    public static class EmailSettings
    {
        private static string _defaultSmtpServer;

        /// <summary>
        /// Gets or sets the default SMTP server used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpServer and OrganizationalDefaultSettings: key = Email.SmtpServer)
        /// </summary>
        public static string DefaultSmtpServer
        {
            get
            {
                return _defaultSmtpServer
                    ?? Config.Get("Horseshoe.NET:Email:SmtpServer");
            }
            set
            {
                _defaultSmtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default SMTP port used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpPort and OrganizationalDefaultSettings: key = Email.SmtpPort)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.Get<int?>("Horseshoe.NET:Email:SmtpPort");
            }
            set
            {
                _defaultPort = value;
            }
        }

        private static bool? _defaultEnableSsl;

        /// <summary>
        /// Gets or sets the SSL setting used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpEnableSsl and OrganizationalDefaultSettings: key = Email.SmtpEnableSsl)
        /// </summary>
        public static bool DefaultEnableSsl
        {
            get
            {
                return _defaultEnableSsl
                    ?? Config.Get<bool>("Horseshoe.NET:Email:EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SMTP login credentials used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpUserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Email.SmtpCredentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:Email:UserName");
                var configPassword = Config.Get("Horseshoe.NET:Email:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:Email:IsEncryptedPassword");
                var configDomain = Config.Get("Horseshoe.NET:Email:Domain");
                return _defaultCredentials
                    ??
                    (
                        configIsEncryptedPassword
                        ? Credential.Build(configUserName, () => Decrypt.String(configPassword), domain: configDomain)
                        : Credential.Build(configUserName, configPassword, domain: configDomain)
                    );
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultFrom;

        /// <summary>
        /// Gets or sets the default sender address used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:From and OrganizationalDefaultSettings: key = Email.From)
        /// </summary>
        public static string DefaultFrom
        {
            get
            {
                return _defaultFrom
                    ?? Config.Get("Horseshoe.NET:Email:From");
            }
            set
            {
                _defaultFrom = value;
            }
        }

        static string _defaultFooterText;

        /// <summary>
        /// Gets or sets the default footer text used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:FooterText and OrganizationalDefaultSettings: key = Email.Footer)
        /// </summary>
        public static string DefaultFooterText
        {
            get
            {
                return _defaultFooterText
                    ?? Config.Get("Horseshoe.NET:Email:Footer");
            }
            set
            {
                _defaultFooterText = value;
            }
        }
    }
}
