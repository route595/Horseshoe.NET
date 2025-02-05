using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

using System.Collections.Generic;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Configuration settings as C# properties
    /// </summary>
    public static class OleDbSettings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default OLEDB connection string name.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:OleDb:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default OLEDB connection string.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = OleDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName), Config.Get<bool>("Horseshoe.NET:OleDb:IsEncryptedPassword"));
            }
        }

        private static string _GetConnectionString(string connectionString, bool isEncryptedPassword)
        {
            if (connectionString == null) return null;
            return isEncryptedPassword
                ? DbUtil.DecryptInlinePassword(connectionString)
                : connectionString;
        }

        /// <summary>
        /// Sets the default OLEDB connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static string _defaultDataSource;

        /// <summary>
        /// Gets or sets the default OLEDB data source.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:DataSource and OrganizationalDefaultSettings: key = OleDb.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource       // e.g. DBSVR01
                    ?? Config.Get("Horseshoe.NET:OleDb:DataSource");
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default OLEDB credentials.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:UserName|Password and OrganizationalDefaultSettings: key = OleDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:OleDb:UserID");
                var configPassword = Config.Get("Horseshoe.NET:OleDb:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:OleDb:IsEncryptedPassword");
                return _defaultCredentials
                    ??
                    (
                        configIsEncryptedPassword
                        ? Credential.Build(configUserName,() => Decrypt.String(configPassword))
                        : Credential.Build(configUserName, configPassword)
                    );
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional OLEDB connection attributes.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = OleDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes        // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? DbUtil.ParseAdditionalConnectionAttributes(Config.Get("Horseshoe.NET:OleDb:AdditionalConnectionAttributes"));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultConnectionTimeout;

        /// <summary>
        /// Gets or sets the default OLEDB connection timeout.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:ConnectionTimeout and OrganizationalDefaultSettings: key = OleDb.ConnectionTimeout)
        /// </summary>
        public static int? DefaultConnectionTimeout
        {
            get
            {
                return _defaultConnectionTimeout           // e.g. 30 (Microsoft default)
                    ?? Config.Get<int?>("Horseshoe.NET:OleDb:ConnectionTimeout");
            }
            set
            {
                _defaultConnectionTimeout = value;
            }
        }

        /// <summary>
        /// Whether to use "Connect Timeout" rather than "Connection Timeout" in generated connection strings. Default is <c>true</c>.
        /// </summary>
        public static bool PreferConnectTimeoutInGeneratedConnectionString { get; set; } = true;

        /// <summary>
        /// Gets or sets the optional cryptographic properties used to decrypt database passwords
        /// </summary>
        public static CryptoOptions CryptoOptions { get; set; }
    }
}
