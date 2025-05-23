﻿using System.Collections.Generic;

using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    public static class SqlDbSettings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default SQL Server connection string name used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:SqlDb:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default SQL Server connection string used by Horseshoe.NET.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = SqlDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName), Config.Get<bool>("Horseshoe.NET:SqlDb:IsEncryptedPassword"));
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
        /// Sets the default SQL Server connection string used by Horseshoe.NET. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static DbServer _defaultDataSource;

        /// <summary>
        /// Gets or sets the default SQL Server data source used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:DataSource and OrganizationalDefaultSettings: key = SqlDb.DataSource)
        /// </summary>
        public static DbServer DefaultDataSource
        {
            get
            {
                return _defaultDataSource         // e.g. DBSVR01
                    ?? DbServer.Parse(Config.Get("Horseshoe.NET:SqlDb:DataSource"));
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static string _defaultInitialCatalog;

        /// <summary>
        /// Gets or sets the default SQL Server initial catalog (database) used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:InitialCatalog and OrganizationalDefaultSettings: key = SqlDb.InitialCatalog)
        /// </summary>
        public static string DefaultInitialCatalog
        {
            get
            {
                return _defaultInitialCatalog           // e.g. CustomerDatabase
                    ?? Config.Get("Horseshoe.NET:SqlDb:InitialCatalog");
            }
            set
            {
                _defaultInitialCatalog = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SQL Server credentials used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:UserName|Password and OrganizationalDefaultSettings: key = SqlDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                var configUserName = Config.Get("Horseshoe.NET:SqlDb:UserID");
                var configPassword = Config.Get("Horseshoe.NET:SqlDb:Password");
                var configIsEncryptedPassword = Config.Get<bool>("Horseshoe.NET:SqlDb:IsEncryptedPassword");
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
        /// Gets or sets the default additional SQL Server connection attributes used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = SqlDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:SqlDb:AdditionalConnectionAttributes", parser: (raw) => DbUtil.ParseAdditionalConnectionAttributes(raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultConnectionTimeout;

        /// <summary>
        /// Gets or sets the default SQL Server timeout used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:Timeout and OrganizationalDefaultSettings: key = SqlDb.Timeout)
        /// </summary>
        public static int? DefaultConnectionTimeout
        {
            get
            {
                return _defaultConnectionTimeout         // e.g. 30 (Microsoft default)
                    ?? Config.Get<int?>("Horseshoe.NET:SqlDb:ConnectionTimeout");
            }
            set
            {
                _defaultConnectionTimeout = value;
            }
        }

        private static IEnumerable<DbServer> _serverList;

        /// <summary>
        /// Gets or sets a list of SQL Servers for DbServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:SqlDb:ServerList and OrganizationalDefaultSettings: key = SqlDb.ServerList)
        /// </summary>
        public static IEnumerable<DbServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =          // e.g. DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2
                        Config.Get("Horseshoe.NET:SqlDb:ServerList", parser: (raw) => DbServer.ParseList(raw));
                }
                return _serverList;
            }
            set
            {
                _serverList = value;
            }
        }

        /// <summary>
        /// Whether to use "Connect Timeout" rather than "Connection Timeout" in generated connection strings
        /// </summary>
        public static bool PreferConnectTimeoutInGeneratedConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the optional cryptographic properties used to decrypt database passwords
        /// </summary>
        public static CryptoOptions CryptoOptions { get; set; }
    }
}
