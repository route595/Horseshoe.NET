using System;

namespace Horseshoe.NET.Email
{
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
                    ?? _Config.Get("Horseshoe.NET:Email:SmtpServer") 
                    ?? OrganizationalDefaultSettings.GetString("Email.SmtpServer");
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
                    ?? _Config.GetNInt("Horseshoe.NET:Email:SmtpPort") 
                    ?? OrganizationalDefaultSettings.GetNInt("Email.SmtpPort");
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
                    ?? _Config.GetNBool("Horseshoe.NET:Email:EnableSsl")
                    ?? OrganizationalDefaultSettings.GetBoolean("Email.EnableSsl");
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
                return _defaultCredentials
                    ?? Credential.Build(_Config.Get("Horseshoe.NET:Email:UserName"), _Config.Get("Horseshoe.NET:Email:Password"), isEncryptedPassword: _Config.GetBool("Horseshoe.NET:Email:IsEncryptedPassword"), domain: _Config.Get("Horseshoe.NET:Email:Domain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Email.Credentials");
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
                    ?? _Config.Get("Horseshoe.NET:Email:From")
                    ?? OrganizationalDefaultSettings.GetString("Email.From");
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
                    ?? _Config.Get("Horseshoe.NET:Email:Footer")
                    ?? OrganizationalDefaultSettings.GetString("Email.Footer");
            }
            set
            {
                _defaultFooterText = value;
            }
        }
    }
}
