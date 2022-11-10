using System;
using System.Collections.Generic;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.OleDb
{
    public static class OleDbSettings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default OLEDB connection string name used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? _Config.Get("Horseshoe.NET:OleDb:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default OLEDB connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = OleDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(_Config.GetConnectionString(DefaultConnectionStringName), _Config.GetBool("Horseshoe.NET:OleDb:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("OleDb.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("OleDb.IsEncryptedPassword"));
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
        /// Gets or sets the default OLEDB data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:DataSource and OrganizationalDefaultSettings: key = OleDb.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource       // e.g. DBSVR01
                    ?? _Config.Get("Horseshoe.NET:OleDb:DataSource")
                    ?? OrganizationalDefaultSettings.GetString("OleDb.DataSource");
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default OLEDB credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:UserName|Password and OrganizationalDefaultSettings: key = OleDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build
                    (
                        _Config.Get("Horseshoe.NET:OleDb:UserID"),
                        _Config.Get("Horseshoe.NET:OleDb:Password"),
                        isEncryptedPassword: _Config.GetBool("Horseshoe.NET:OleDb:IsEncryptedPassword")
                    )
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("OleDb.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional OLEDB connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = OleDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes        // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? _Config.Get("Horseshoe.NET:OleDb:AdditionalConnectionAttributes", parseFunc: (raw) => DbUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? OrganizationalDefaultSettings.Get("OleDb.AdditionalConnectionAttributes", parseFunc: (raw) => DbUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the default OLEDB timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OleDb:Timeout and OrganizationalDefaultSettings: key = OleDb.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout           // e.g. 30 (Microsoft default?)
                    ?? _Config.GetNInt("Horseshoe.NET:OleDb:Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("OleDb.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }
    }
}
