using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.SecureIO.Sftp
{
    public static class SftpSettings
    {
        private static string _defaultFtpServer;

        /// <summary>
        /// Gets or sets the default SFTP server.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp:Server and OrganizationalDefaultSettings: key = Sftp.Server)
        /// </summary>
        public static string DefaultFtpServer
        {
            get
            {
                return _defaultFtpServer
                    ?? Config.Get("Horseshoe.NET:Sftp:Server") 
                    ?? OrganizationalDefaultSettings.Get<string>("Sftp.Server");
            }
            set
            {
                _defaultFtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default SFTP port.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp:Port and OrganizationalDefaultSettings: key = Sftp.Port)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.Get<int?>("Horseshoe.NET:Sftp:Port") 
                    ?? OrganizationalDefaultSettings.Get<int?>("Sftp.Port");
            }
            set
            {
                _defaultPort = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default credentials used by SFTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp:UserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Sftp.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:Sftp:UserName");
                var configPassword = Config.Get("Horseshoe.NET:Sftp:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:Sftp:IsEncryptedPassword");
                return _defaultCredentials
                    ??
                    (
                        configIsEncryptedPassword
                        ? Credential.Build(configUserName, () => Decrypt.String(configPassword))
                        : Credential.Build(configUserName, configPassword)
                    )
                    ?? OrganizationalDefaultSettings.Get<Credential?>("Sftp.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultServerPath;

        /// <summary>
        /// Gets or sets the default server path used by SFTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp:ServerPath and OrganizationalDefaultSettings: key = Sftp.ServerPath)
        /// </summary>
        public static string DefaultServerPath
        {
            get
            {
                return _defaultServerPath
                    ?? Config.Get("Horseshoe.NET:Sftp:ServerPath")
                    ?? "";
            }
            set
            {
                _defaultServerPath = value;
            }
        }
    }
}
