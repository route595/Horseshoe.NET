﻿using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.IO.Ftp
{
    /// <summary>
    /// Configuration settings for Horseshoe.NET.Ftp
    /// </summary>
    public static class FtpSettings
    {
        private static string _defaultFtpServer;

        /// <summary>
        /// Gets or sets the default FTP server.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp:Server and OrganizationalDefaultSettings: key = Ftp.Server)
        /// </summary>
        public static string DefaultFtpServer
        {
            get
            {
                return _defaultFtpServer
                    ?? Config.Get("Horseshoe.NET:Ftp:Server");
            }
            set
            {
                _defaultFtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default FTP port.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp:Port and OrganizationalDefaultSettings: key = Ftp.Port)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.Get<int?>("Horseshoe.NET:Ftp:Port");
            }
            set
            {
                _defaultPort = value;
            }
        }

        private static bool? _defaultEnableSsl;

        /// <summary>
        /// Gets or sets whether FTP will use SSL.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp:EnableSsl and OrganizationalDefaultSettings: key = Ftp.EnableSsl)
        /// </summary>
        public static bool DefaultEnableSsl
        {
            get
            {
                return _defaultEnableSsl
                    ?? Config.Get<bool>("Horseshoe.NET:Ftp:EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default credentials used by FTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp:UserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Ftp.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:Ftp:UserName");
                var configPassword = Config.Get("Horseshoe.NET:Ftp:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:Ftp:IsEncryptedPassword");
                var configDomain = Config.Get("Horseshoe.NET:Ftp:Domain");
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

        static string _defaultServerPath;

        /// <summary>
        /// Gets or sets the default server path used by FTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp:ServerPath and OrganizationalDefaultSettings: key = Ftp.ServerPath)
        /// </summary>
        public static string DefaultServerPath
        {
            get
            {
                return _defaultServerPath
                    ?? Config.Get("Horseshoe.NET:Ftp:ServerPath")
                    ?? "";
            }
            set
            {
                _defaultServerPath = value;
            }
        }
    }
}
