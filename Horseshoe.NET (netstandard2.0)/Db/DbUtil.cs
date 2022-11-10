using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    public static class DbUtil
    {
        /* * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION STRINGS + CREDENTIALS    *
         * * * * * * * * * * * * * * * * * * * * */

        public static T LoadConnectionInfo<T>(T connectionInfo, Func<string> buildConnectionStringFromConfig) where T : ConnectionInfo
        {
            // first check 'user-supplied' info
            if (connectionInfo != null)
            {
                connectionInfo = (T)connectionInfo.Clone();
                if (connectionInfo.ConnectionString != null)
                {
                    connectionInfo.Source = "user-supplied-connection-string";
                    return connectionInfo;
                }

                if (connectionInfo.ConnectionStringName != null)
                {
                    connectionInfo.ConnectionString = _Config.GetConnectionString(connectionInfo.ConnectionStringName)
                        ?? throw new UtilityException("Cannot find connection string named '" + connectionInfo.ConnectionStringName + "'.  Please ensure Horseshoe.NET.Configuration is installed.");
                    connectionInfo.Source = "user-supplied-connection-string-name";
                    return connectionInfo;
                }

                if (connectionInfo.DataSource != null)
                {
                    connectionInfo.ConnectionString = connectionInfo.BuildConnectionString()
                        ?? throw new UtilityException("Cannot build connection string from data source '" + connectionInfo.DataSource + "'.");
                    connectionInfo.Source = "user-supplied-data-source";
                    return connectionInfo;
                }
            }

            // next, check config
            var configPart = typeof(T).Name.Replace("ConnectionInfo", "");

            var connStr = _Config.Get("Horseshoe.NET:" + configPart + ":ConnectionString");
            if (connStr != null)
            {
                connectionInfo.ConnectionString = connStr;
                connectionInfo.Credentials = Credential.Build
                (
                    _Config.Get("Horseshoe.NET:" + configPart + ":UserID"),
                    _Config.Get("Horseshoe.NET:" + configPart + ":Password"),
                    _Config.GetBool("Horseshoe.NET:" + configPart + ":IsEncryptedPassword")
                );
                connectionInfo.Source = "config-connection-string";
                return connectionInfo;
            }

            var connStrName = _Config.Get("Horseshoe.NET:" + configPart + ":ConnectionStringName");
            if (connStrName != null)
            {
                connectionInfo.ConnectionString = _Config.GetConnectionString(connStrName)
                    ?? throw new UtilityException("Cannot find connection string named '" + connStrName + "'.");
                connectionInfo.Credentials = Credential.Build
                (
                    _Config.Get("Horseshoe.NET:" + configPart + ":UserID"),
                    _Config.Get("Horseshoe.NET:" + configPart + ":Password"),
                    _Config.GetBool("Horseshoe.NET:" + configPart + ":IsEncryptedPassword")
                );
                connectionInfo.Source = "config-connection-string-name";
                return connectionInfo;
            }

            if (buildConnectionStringFromConfig != null)
            {
                connStr = buildConnectionStringFromConfig();
                if (connStr != null)
                {
                    connectionInfo.ConnectionString = connStr;
                    connectionInfo.Credentials = Credential.Build
                    (
                        _Config.Get("Horseshoe.NET:" + configPart + ":UserID"),
                        _Config.Get("Horseshoe.NET:" + configPart + ":Password"),
                        _Config.GetBool("Horseshoe.NET:" + configPart + ":IsEncryptedPassword")
                    );
                    connectionInfo.Source = "config-data-source";
                    return connectionInfo;
                }
            }

            throw new UtilityException("Cannot find any connection info.  Please ensure NuGet package Horseshoe.NET.Configuration is installed.");
        }

        public static string DecryptInlinePassword(string connStrWithEcryptedPassword, CryptoOptions options = null)
        {
            var cipherText = ParseConnectionStringValue(ConnectionStringPart.Password, connStrWithEcryptedPassword);
            if (cipherText == null)
                return null;
            var plainText = Decrypt.String(cipherText, options: options);
            var reconstitutedConnStr = connStrWithEcryptedPassword.Replace(cipherText, plainText);
            return reconstitutedConnStr;
        }

        public static string HideInlinePassword(string connectionString)
        {
            var plainText = ParseConnectionStringValue(ConnectionStringPart.Password, connectionString);
            if (plainText == null) return connectionString;
            var reconstitutedConnStr = connectionString.Replace(plainText, "******");
            return reconstitutedConnStr;
        }

        public static string ParseConnectionStringValue(string key, string connectionString)
        {
            var match = new Regex("(?<=" + key + "=)[^;]+", RegexOptions.IgnoreCase).Match(connectionString);
            return Zap.String(match.Value);
        }

        public static string ParseConnectionStringValue(ConnectionStringPart part, string connectionString)
        {
            switch (part)
            {
                case ConnectionStringPart.DataSource:
                    return ParseConnectionStringValue("Data Source", connectionString) ?? ParseConnectionStringValue("Server", connectionString) ?? ParseConnectionStringValue("DSN", connectionString);
                case ConnectionStringPart.InitialCatalog:
                    return ParseConnectionStringValue("Initial Catalog", connectionString) ?? ParseConnectionStringValue("Database", connectionString);
                case ConnectionStringPart.UserId:
                    return ParseConnectionStringValue("User ID", connectionString) ?? ParseConnectionStringValue("UID", connectionString);
                case ConnectionStringPart.Password:
                    return ParseConnectionStringValue("Password", connectionString) ?? ParseConnectionStringValue("PWD", connectionString);
            }
            throw new UtilityException("Unknown connection string part: " + part);  // this should never happen
        }

        public static IDictionary<string, string> ParseAdditionalConnectionAttributes(string text)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(text))
                return dict;
            var list = Zap.Strings(text.Split('|'), prunePolicy: PrunePolicy.All);
            foreach (var attr in list)
            {
                var attrParts = attr.Split('=').Trim();
                if (attrParts.Length == 2)
                {
                    dict.Add(attrParts[0], attrParts[1]);
                }
                else
                {
                    throw new UtilityException("Invalid additional connection string attribute: " + attr);
                }
            }
            return dict;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *         TABLES AND COLUMNS          *
         * * * * * * * * * * * * * * * * * * * */
        static readonly Regex SimpleColumnNamePattern = new Regex("^[A-Z0-9_]+$", RegexOptions.IgnoreCase);

        public static string RenderColumnName(DbParameter parameter, DbPlatform? platform = null)
        {
            if (Zap.String(parameter.ParameterName) == null)
                throw new ValidationException("column name cannot be null");
            return RenderColumnName(parameter.ParameterName, platform: platform);
        }

        public static string RenderColumnName(string columnName, DbPlatform? platform = null)
        {
            if (SimpleColumnNamePattern.IsMatch(columnName))
                return columnName;
            switch (platform)
            {
                case DbPlatform.SqlServer:
                    return "[" + columnName + "]";
                default:
                    return "\"" + columnName + "\"";
            }
        }

        public static void TrimDataTable(DataTable dataTable)
        {
            var fieldTypes = dataTable.Columns
                .Cast<DataColumn>()
                .Select(dc => dc.DataType)
                .ToArray();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow.ItemArray.Length != fieldTypes.Length)
                {
                    throw new UtilityException("Row items do not match field types: " + dataRow.ItemArray.Length + ", " + fieldTypes.Length);
                }
                for (int i = 0; i < dataRow.ItemArray.Length; i++)
                {
                    if (dataRow[i] is string stringValue)
                    {
                        dataRow[i] = stringValue.Trim();
                    }
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *     OBJECT / RESULT SET HELPERS     *
         * * * * * * * * * * * * * * * * * * * */
        public static object[] GetAllValues(IDataReader reader, int columnCount, AutoTruncate autoTrunc = default)
        {
            var items = new object[columnCount];

            reader.GetValues(items);

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = NormalizeDbValue(items[i]);
                if (items[i] is string stringValue)
                {
                    switch (autoTrunc)
                    {
                        case AutoTruncate.Trim:
                            items[i] = stringValue.Trim();
                            break;
                        case AutoTruncate.Zap:
                            items[i] = Zap.String(stringValue);
                            break;
                        case AutoTruncate.ZapEmptyStringsOnly:
                            if (stringValue.Length == 0)
                                items[i] = null;
                            break;
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Prepare an object for insertion into a SQL statement
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="platform">A vendor-specific DB platform</param>
        /// <returns></returns>
        public static string Sqlize(object obj, DbPlatform? platform = null)
        {
            if (obj == null || obj is DBNull) return "NULL";
            if (obj is bool boolValue) obj = boolValue ? 1 : 0;
            if (obj is byte || obj is short || obj is int || obj is long || obj is float || obj is double || obj is decimal) return obj.ToString();
            if (obj is DateTime dateTimeValue)
            {
                string dateTimeFormat;
                switch (platform ?? DbPlatform.Neutral)
                {
                    case DbPlatform.SqlServer:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";  // format inspired by Microsoft Sql Server Managament Studio
                        break;
                    case DbPlatform.Oracle:
                        dateTimeFormat = "MM/dd/yyyy hh:mm:ss tt";   // format inspired by dbForge for Oracle
                        var oracleDateFormat = "mm/dd/yyyy hh:mi:ss am";
                        return "TO_DATE('" + dateTimeValue.ToString(dateTimeFormat) + "', '" + oracleDateFormat + "')";  // example: TO_DATE('02/02/2014 3:35:57 PM', 'mm/dd/yyyy hh:mi:ss am')
                    default:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                        break;
                }
                return "'" + dateTimeValue.ToString(dateTimeFormat) + "'";
            }
            if (obj is SqlLiteral sqlLiteral)
            {
                return sqlLiteral.Expression;
            }
            var returnValue = "'" + obj.ToString().Replace("'", "''") + "'";
            if (platform == DbPlatform.SqlServer && !TextUtil.IsASCIIPrintable(returnValue))
            {
                return "N" + returnValue;
            }
            return returnValue;
        }

        public static object NormalizeDbValue(object obj)
        {
            if (obj == null || obj is DBNull)
                return null;
            return obj;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *           MISCELLANEOUS             *
         * * * * * * * * * * * * * * * * * * * */
        internal static object WrangleParameterValue(object value)
        {
            if (value == null)
                return DBNull.Value;
            if (value.GetType().IsEnum)
                return value.ToString();
            return value;
        }

        internal static DbType CalculateDbType(object value)
        {
            if (value is string) return DbType.String;
            if (value is byte) return DbType.Byte;
            if (value is short) return DbType.Int16;
            if (value is int) return DbType.Int32;
            if (value is long) return DbType.Int64;
            if (value is decimal) return DbType.Decimal;
            if (value is double) return DbType.Double;
            if (value is bool) return DbType.Boolean;
            if (value is DateTime) return DbType.DateTime;
            if (value is Guid) return DbType.Guid;
            return DbType.Object;
        }
    }
}
