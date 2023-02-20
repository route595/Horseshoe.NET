using System.Collections.Generic;

using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb.Meta;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class OracleDbSettings
    {
        public static string SqlNetAuthenticationServices
        {
            get => OracleConfiguration.SqlNetAuthenticationServices;
            set => OracleConfiguration.SqlNetAuthenticationServices = value;
        }

        /// <summary>
        /// Gets the default Oracle auth method used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ConnectionStringName)
        /// </summary>
        public static string DefaultSqlNetAuthenticationServices =>
            Config.Get("Horseshoe.NET:OracleDb:SqlNetAuthenticationServices");

        static OracleDbSettings()
        {
            var authSvc = DefaultSqlNetAuthenticationServices;
            if (authSvc != null)
            {
                SqlNetAuthenticationServices = authSvc;
            }
        }

        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default Oracle connection string name used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:OracleDb:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default Oracle connection string used by Horseshoe.NET.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = OracleDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName), Config.Get<bool>("Horseshoe.NET:OracleDb:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.Get<string>("OracleDb.ConnectionString"), OrganizationalDefaultSettings.Get<bool>("OracleDb.IsEncryptedPassword"));
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
        /// Sets the default Oracle connection string used by Horseshoe.NET. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static OraServer _defaultDataSource;

        /// <summary>
        /// Gets or sets the default Oracle server used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:Server and OrganizationalDefaultSettings: key = OracleDb.Server)
        /// </summary>
        public static OraServer DefaultDataSource
        {
            get
            {
                if (_defaultDataSource == null)
                {
                    _defaultDataSource =      // e.g. ORADBSVR01 or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:OracleDb:DataSource", parseFunc: (raw) => OraServer.Parse(raw)) ??
                        OrganizationalDefaultSettings.Get<OraServer>("OracleDb.DataSource"); 
                }
                return _defaultDataSource;
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        //private static string _defaultDataSource;

        ///// <summary>
        ///// Gets or sets the default Oracle datasource used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:DataSource and OrganizationalDefaultSettings: key = OracleDb.DataSource)
        ///// </summary>
        //public static string DefaultDataSource
        //{
        //    get
        //    {
        //        return _defaultDataSource     // e.g. ORADBSVR01
        //            ?? Config.Get("Horseshoe.NET:OracleDb:DataSource")
        //            ?? OrganizationalDefaultSettings.Get<string>("OracleDb.DataSource")
        //            ?? DefaultServer?.DataSource;
        //    }
        //    set
        //    {
        //        _defaultDataSource = value;
        //    }
        //}

        private static string _defaultServiceName;

        /// <summary>
        /// Gets or sets the default Oracle service name used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ServiceName and OrganizationalDefaultSettings: key = OracleDb.ServiceName)
        /// </summary>
        public static string DefaultServiceName
        {
            get
            {
                return _defaultServiceName     // e.g. MYDATABASE
                    ?? Config.Get("Horseshoe.NET:OracleDb:ServiceName")
                    ?? OrganizationalDefaultSettings.Get<string>("OracleDb.ServiceName")
                    ?? DefaultDataSource?.DataSource;
            }
            set
            {
                _defaultServiceName = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default Oracle credentials used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:UserID|Password and OrganizationalDefaultSettings: key = OracleDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:OracleDb:UserID");
                var configPassword = Config.Get("Horseshoe.NET:OracleDb:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:OracleDb:IsEncryptedPassword");
                return _defaultCredentials
                    ??
                    (
                        configIsEncryptedPassword
                        ? Credential.Build(configUserName, () => Decrypt.String(configPassword))
                        : Credential.Build(configUserName, configPassword)
                    )
                    ?? OrganizationalDefaultSettings.Get<Credential?>("OracleDb.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the additional Oracle connection string attributes used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = OracleDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:OracleDb:AdditionalConnectionAttributes", parseFunc: (raw) => DbUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? DbUtil.ParseAdditionalConnectionAttributes(OrganizationalDefaultSettings.Get<string>("OracleDb.AdditionalConnectionAttributes"));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultConnectionTimeout;

        /// <summary>
        /// Gets or sets the Oracle timeout used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:Timeout and OrganizationalDefaultSettings: key = OracleDb.Timeout)
        /// </summary>
        public static int? DefaultConnectionTimeout
        {
            get
            {
                return _defaultConnectionTimeout
                    ?? Config.Get<int?>("Horseshoe.NET:OracleDb:ConnectionTimeout")
                    ?? OrganizationalDefaultSettings.Get<int?>("OracleDb.ConnectionTimeout");
            }
            set
            {
                _defaultConnectionTimeout = value;
            }
        }

        private static int? _reattemptInterval;

        /// <summary>
        /// Gets or sets the Oracle reattempt interval used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ReattemptInterval and OrganizationalDefaultSettings: key = OracleDb.ReattemptInterval)
        /// </summary>
        public static int ReattemptInterval
        {
            get
            {
                return _reattemptInterval
                    ?? Config.Get<int?>("Horseshoe.NET:OracleDb:ReattemptInterval")
                    ?? OrganizationalDefaultSettings.Get<int?>("OracleDb.ReattemptInterval")
                    ?? 500;
            }
            set
            {
                _reattemptInterval = value;
            }
        }

        private static bool? _defaultAutoClearConnectionPool;

        /// <summary>
        /// Gets or sets whether Horseshoe.NET instructs connections to clear their pool upon closing.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:AutoClearConnectionPool and OrganizationalDefaultSettings: key = OracleDb.AutoClearConnectionPool)
        /// </summary>
        public static bool AutoClearConnectionPool
        {
            // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
            get
            {
                return _defaultAutoClearConnectionPool
                    ?? Config.Get<bool?>("Horseshoe.NET:OracleDb:AutoClearConnectionPool")
                    ?? OrganizationalDefaultSettings.Get<bool?>("OracleDb.AutoClearConnectionPool")
                    ?? false;
            }
            set
            {
                _defaultAutoClearConnectionPool = value;
            }
        }

        private static IEnumerable<OraServer> _serverList;

        /// <summary>
        /// Gets or sets a list of Oracle servers for OraServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ServerList and OrganizationalDefaultSettings: key = OracleDb.ServerList)
        /// </summary>
        public static IEnumerable<OraServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =      // e.g. ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:OracleDb:ServerList", parseFunc: (raw) => OraServer.ParseList(raw)) ??
                        OraServer.ParseList(OrganizationalDefaultSettings.Get<string>("OracleDb.ServerList"));
                }
                return _serverList;
            }
            set
            {
                _serverList = value;
            }
        }
    }
}
