﻿using System.Collections.Generic;

using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

namespace Horseshoe.NET.Odbc
{
    public static class OdbcSettings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default ODBC connection string name.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Odbc:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:Odbc:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default ODBC connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = Odbc.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName), Config.Get<bool>("Horseshoe.NET:Odbc:IsEncryptedPassword"));
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
        /// Sets the default ODBC connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static string _defaultDataSource;

        /// <summary>
        /// Gets or sets the default ODBC data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Odbc:DataSource and OrganizationalDefaultSettings: key = Odbc.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource     // e.g. DBSVR01
                    ?? Config.Get("Horseshoe.NET:Odbc:DataSource");
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default ODBC credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Odbc:UserName|Password and OrganizationalDefaultSettings: key = Odbc.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:Odbc:UserID");
                var configPassword = Config.Get("Horseshoe.NET:Odbc:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:Odbc:IsEncryptedPassword");
                return _defaultCredentials
                    ??
                    (
                        configIsEncryptedPassword
                        ? Credential.Build(configUserName, () => Decrypt.String(configPassword))
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
        /// Gets or sets the default additional ODBC connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Odbc:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = Odbc.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes      // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:Odbc:AdditionalConnectionAttributes", parser: (raw) => DbUtil.ParseAdditionalConnectionAttributes(raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultConnectionTimeout;

        /// <summary>
        /// Gets or sets the default ODBC timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Odbc:Timeout and OrganizationalDefaultSettings: key = Odbc.Timeout)
        /// </summary>
        public static int? DefaultConnectionTimeout
        {
            get
            {
                return _defaultConnectionTimeout       // e.g. 30 (Microsoft default)
                    ?? Config.Get<int?>("Horseshoe.NET:Odbc:ConnectionTimeout");
            }
            set
            {
                _defaultConnectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets optional cryptographic properties used to decrypt database passwords
        /// </summary>
        public static CryptoOptions CryptoOptions { get; set; }
    }
}
