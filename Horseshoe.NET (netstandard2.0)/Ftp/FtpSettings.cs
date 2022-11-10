using System;

namespace Horseshoe.NET.Ftp
{
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
                    ?? _Config.Get("Horseshoe.NET:Ftp:Server") 
                    ?? OrganizationalDefaultSettings.GetString("Ftp.Server");
            }
            set
            {
                _defaultFtpServer = value;
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
                    ?? _Config.GetNBool("Horseshoe.NET:Ftp:EnableSsl")
                    ?? OrganizationalDefaultSettings.GetBoolean("Ftp.EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
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
                    ?? _Config.GetNInt("Horseshoe.NET:Ftp:Port")
                    ?? OrganizationalDefaultSettings.GetNInt("Ftp.Port");
            }
            set
            {
                _defaultPort = value;
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
                return _defaultCredentials
                    ?? Credential.Build(_Config.Get("Horseshoe.NET:Ftp:UserName"), _Config.Get("Horseshoe.NET:Ftp:Password"), isEncryptedPassword: _Config.GetBool("Horseshoe.NET:Ftp:IsEncryptedPassword"), domain: _Config.Get("Horseshoe.NET:Ftp:Domain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Ftp.Credentials");
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
                    ?? _Config.Get("Horseshoe.NET:Ftp:ServerPath")
                    ?? "";
            }
            set
            {
                _defaultServerPath = value;
            }
        }
    }
}
