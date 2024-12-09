using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A collection of common, provider agnostic factory methods for Horseshoe.NET DB operations.
    /// </summary>
    public static class DbUtil
    {
        /* * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION STRINGS + CREDENTIALS    *
         * * * * * * * * * * * * * * * * * * * * */

        ///// <summary>
        ///// Loads connection info from all possible sources, reporting which source contained the info,
        ///// and compiles it all into a resultant <c>ConnectionInfo</c> instance
        ///// </summary>
        ///// <typeparam name="T">a subclass of <c>ConnectionInfo</c></typeparam>
        ///// <param name="connectionInfo">an optional <c>ConnectionInfo</c> instance</param>
        ///// <param name="buildConnectionStringFromConfig">an optional function for building a provider-specific connection string from <c>app|web.config</c> or <c>appsettings.json</c></param>
        ///// <param name="cryptoOptions">Optional options for the crypto engine to decrypt the connection string password. Source should be DB settings.</param>
        ///// <param name="getConnectionStringSource">Sends the connection string source to any listening client</param>
        ///// <param name="journal">A trace journal to which each step of the process is logged.</param>
        ///// <returns>A new <c>ConnectionInfo</c> object whose <c>ConnectionString</c> should be used to build connections.</returns>
        ///// <exception cref="ValidationException"></exception>
        //public static T LoadConnectionInfo<T>(T connectionInfo, Func<string> buildConnectionStringFromConfig, CryptoOptions cryptoOptions = null, Func<string> getConnectionStringSource = null, TraceJournal journal = null) where T : ConnectionInfo, new()
        //{
        //    // journaling
        //    journal = journal ?? new TraceJournal();
        //    journal.WriteEntry("DbUtil.LoadConnectionInfo()  =>  T = " + typeof(T).FullName);
        //    journal.Level++;

        //    var configPart = typeof(T).Name.Replace("ConnectionInfo", "");
        //    var configUserName = _Config.Get("Horseshoe.NET:" + configPart + ":UserID");
        //    var configPassword = _Config.Get("Horseshoe.NET:" + configPart + ":Password");
        //    var configIsEncryptedPassword = _Config.Get<bool>("Horseshoe.NET:" + configPart + ":IsEncryptedPassword");

        //    // first check 'user-supplied' info
        //    if (connectionInfo != null)
        //    {
        //        connectionInfo = (T)connectionInfo.Clone();
        //        if (connectionInfo.ConnectionString != null)
        //        {
        //            journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //            journal.AddAndWriteEntry("connection.info.source", "user-supplied-connection-string");
        //            if (connectionInfo.IsEncryptedPassword)
        //            {
        //                connectionInfo.ConnectionString = DecryptInlinePassword(connectionInfo.ConnectionString, connectionInfo.CryptoOptions ?? cryptoOptions);
        //                connectionInfo.IsEncryptedPassword = false;
        //            }
        //            journal.Level--;
        //            return connectionInfo;
        //        }

        //        if (connectionInfo.ConnectionStringName != null)
        //        {
        //            journal.WriteEntry("connection string name = " + connectionInfo.ConnectionStringName);
        //            connectionInfo.ConnectionString = _Config.GetConnectionString(connectionInfo.ConnectionStringName);
        //            if (connectionInfo.ConnectionString == null)
        //            {
        //                var msg = string.Concat("No connection string named \"", connectionInfo.ConnectionStringName, "\" could be found", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " perhaps due to Horseshoe.NET.Configuration is not installed" : "", ".");
        //                journal.WriteEntry("ValidationException: " + msg);
        //                throw new ValidationException(msg);
        //            }
        //            journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //            journal.AddAndWriteEntry("connection.info.source", "user-supplied-connection-string-name");
        //            if (connectionInfo.IsEncryptedPassword || configIsEncryptedPassword)
        //            {
        //                connectionInfo.ConnectionString = DecryptInlinePassword(connectionInfo.ConnectionString, connectionInfo.CryptoOptions ?? cryptoOptions);
        //                connectionInfo.IsEncryptedPassword = false;
        //            }
        //            journal.Level--;
        //            return connectionInfo;
        //        }

        //        if (connectionInfo.DataSource != null)
        //        {
        //            journal.WriteEntry("data source = " + connectionInfo.DataSource);
        //            connectionInfo.ConnectionString = connectionInfo.BuildConnectionString();
        //            if (connectionInfo.ConnectionString == null)
        //            {
        //                var msg = string.Concat("Cannot build connection string from data source: \"", connectionInfo.DataSource, "\".", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " This is perhaps due to Horseshoe.NET.Configuration is not installed." : "");
        //                journal.WriteEntry("ValidationException: " + msg);
        //                throw new ValidationException(msg);
        //            }
        //            journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //            journal.AddAndWriteEntry("connection.info.source", "user-supplied-data-source");
        //            journal.Level--;
        //            return connectionInfo;
        //        }
        //    }
        //    else
        //    {
        //        connectionInfo = new T();
        //    }

        //    // next, check config
        //    var connStr = _Config.Get("Horseshoe.NET:" + configPart + ":ConnectionString");
        //    if (connStr != null)
        //    {
        //        connectionInfo.ConnectionString = connStr;
        //        connectionInfo.Credentials = configIsEncryptedPassword
        //            ? Credential.Build(configUserName, () => Decrypt.String(configPassword))
        //            : Credential.Build(configUserName, configPassword);
        //        journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //        journal.AddAndWriteEntry("connection.info.source", "config-connection-string");
        //        journal.Level--;
        //        return connectionInfo;
        //    }

        //    var connStrName = _Config.Get("Horseshoe.NET:" + configPart + ":ConnectionStringName");
        //    if (connStrName != null)
        //    {
        //        journal.WriteEntry("connection string name = " + connStrName);
        //        connectionInfo.ConnectionString = _Config.GetConnectionString(connStrName);
        //        if (connectionInfo.ConnectionString == null)
        //        {
        //            var msg = string.Concat("No connection string named \"", connStrName, "\" could be found", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " perhaps due to Horseshoe.NET.Config is not installed" : "", ".");
        //            journal.WriteEntry("ValidationException: " + msg);
        //            throw new ValidationException(msg);
        //        }
        //        connectionInfo.Credentials = configIsEncryptedPassword
        //            ? Credential.Build
        //            (
        //                configUserName,
        //                () => Decrypt.String(configPassword)
        //            )
        //            : Credential.Build
        //            (
        //                configUserName,
        //                configPassword
        //            );
        //        journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //        journal.AddAndWriteEntry("connection.info.source", "config-connection-string-name");
        //        journal.Level--;
        //        return connectionInfo;
        //    }

        //    if (buildConnectionStringFromConfig != null)
        //    {
        //        connStr = buildConnectionStringFromConfig();
        //        if (connStr != null)
        //        {
        //            connectionInfo.ConnectionString = connStr;
        //            connectionInfo.Credentials = configIsEncryptedPassword
        //                ? Credential.Build
        //                  (
        //                    configUserName,
        //                    () => Decrypt.String(configPassword)
        //                  )
        //                : Credential.Build
        //                  (
        //                    configUserName,
        //                    configPassword
        //                  );
        //            journal.AddAndWriteEntry("connection.string", HideInlinePassword(connectionInfo.ConnectionString));
        //            journal.AddAndWriteEntry("connection.info.source", "config-data-source");
        //            journal.Level--;
        //            return connectionInfo;
        //        }
        //    }

        //    var dfltMsg = string.Concat("No connection info could be found", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " perhaps due to Horseshoe.NET.Configuration is not installed" : "", ".");
        //    journal.WriteEntry("ValidationException: " + dfltMsg);
        //    throw new ValidationException(dfltMsg);
        //}


        /// <summary>
        /// Internal use only.  Scans all possible sources for the connection string.  Client code
        /// can listen to <c>getConnectionStringSource</c> to get the connection string source.
        /// </summary>
        /// <param name="connectionInfo">The <c>ConnectionInfo</c> supplied to the database method, can be <c>null</c></param>
        /// <param name="buildConnectionStringFromConfig">A provider specific function for building a connection string from configuration file</param>
        /// <param name="cryptoOptions">Optional options for the crypto engine to decrypt the connection string password. Source should be DB settings.</param>
        /// <param name="revealConnectionStringSource">Sends the connection string source to any listening client</param>
        /// <param name="journal">An optional trace journal to which each step of the process is logged.</param>
        /// <returns>A new <c>ConnectionInfo</c> object whose <c>ConnectionString</c> should be used to build connections.</returns>
        /// <exception cref="ValidationException"></exception>
        public static T LoadFinalConnectionInfo<T>(T connectionInfo, Func<string> buildConnectionStringFromConfig, CryptoOptions cryptoOptions = null, Action<string> revealConnectionStringSource = null, TraceJournal journal = null) where T : ConnectionInfo, new()
        {
            // journaling
            journal?.WriteEntry("DbUtil.LoadFinalConnectionInfo()  =>  T = " + typeof(T).FullName);
            journal?.IncrementLevel();

            string connStr = null;
            string connStrSource = null;
            var configPart = typeof(T).Name.Replace("ConnectionInfo", "");
            //var configUserName = _Config.Get("Horseshoe.NET:" + configPart + ":UserID");
            //var configPassword = _Config.Get("Horseshoe.NET:" + configPart + ":Password");
            //var configIsEncryptedPassword = _Config.Get<bool>("Horseshoe.NET:" + configPart + ":IsEncryptedPassword");

            // first check user-supplied connection values
            if (connectionInfo != null)
            {
                // check connection string
                if (connectionInfo.ConnectionString is string _connStr && !string.IsNullOrEmpty(_connStr))
                {
                    connStr = _connStr;
                    connStrSource = "client-supplied-connection-string";
                }
                // then check connection string name
                else if (connectionInfo.ConnectionStringName is string connStrName && !string.IsNullOrEmpty(connStrName) && _Config.GetConnectionString(connStrName) is string _connStrN && !string.IsNullOrEmpty(_connStrN))
                {
                    connStr = _connStrN;
                    connStrSource = "client-supplied-connection-string-name";
                }
                // then check connection string builder values
                else if (connectionInfo.BuildFinalConnectionString() is string _connStrB && !string.IsNullOrEmpty(_connStrB))
                {
                    connStr = _connStrB;
                    connStrSource = "client-supplied-datasource";
                }

                if (connStr != null)
                {
                    var finalConnectionInfo = new T
                    {
                        ConnectionString = connStr,
                        Credentials = connectionInfo.Credentials
                    };
                    journal?.AddAndWriteEntry("connectionstring", HideInlinePassword(connStr));
                    journal?.AddAndWriteEntry("connectionstring.source", connStrSource);
                    revealConnectionStringSource?.Invoke(connStrSource);
                    journal?.DecrementLevel();
                    return finalConnectionInfo;
                }
            }

            // finally, check config

            // check config connection string
            if (_Config.Get("Horseshoe.NET:" + configPart + ":ConnectionString") is string _connStrC && !string.IsNullOrEmpty(_connStrC))
            {
                connStr = _connStrC;
                connStrSource = "config-connection-string";
            }
            // then check config connection string name
            else if (_Config.Get("Horseshoe.NET:" + configPart + ":ConnectionStringName") is string connStrName && !string.IsNullOrEmpty(connStrName) && _Config.GetConnectionString(connStrName) is string _connStrN && !string.IsNullOrEmpty(_connStrN))
            {
                connStr = _connStrN;
                connStrSource = "config-connection-string-name";
            }
            // then check config connection string builder values
            else if (buildConnectionStringFromConfig?.Invoke() is string _connStrB && !string.IsNullOrEmpty(_connStrB))
            {
                connStr = _connStrB;
                connStrSource = "config-datasource";
            }

            if (connStr != null)
            {
                var finalConnectionInfo = new T
                {
                    ConnectionString = connStr,
                    Credentials = BuildCredentials(_Config.Get("Horseshoe.NET:" + configPart + ":UserID"),
                                                   _Config.Get("Horseshoe.NET:" + configPart + ":Password"),
                                                   _Config.Get<bool>("Horseshoe.NET:" + configPart + ":IsEncryptedPassword"),
                                                   cryptoOptions: cryptoOptions)
                };
                journal?.AddAndWriteEntry("connectionstring", HideInlinePassword(connStr));
                journal?.AddAndWriteEntry("connectionstring.source", connStrSource);
                revealConnectionStringSource?.Invoke(connStrSource);
                journal?.DecrementLevel();
                return finalConnectionInfo;
            }

            var msg = string.Concat("No connection info could be found", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " perhaps due to Horseshoe.NET.Configuration is not installed" : "", ".");
            journal?.WriteEntry("ValidationException: " + msg);
            throw new ValidationException(msg);
        }

        /// <summary>
        /// Utility method for inline decrypting the password in a connection string
        /// </summary>
        /// <param name="connectionString">A connection string potentially with an encrypted password</param>
        /// <param name="cryptoOptions">Optional cryptographic properties used to decrypt database passwords</param>
        /// <returns>A sensitive connection string i.e. may contain user id and password.</returns>
        public static string DecryptInlinePassword(string connectionString, CryptoOptions cryptoOptions = null)
        {
            if (connectionString == null)
                return string.Empty;
            var prestart = Math.Max
            (
                connectionString.IndexOf(";password=", StringComparison.OrdinalIgnoreCase),
                connectionString.IndexOf(";pwd=", StringComparison.OrdinalIgnoreCase)
            );
            if (prestart > -1)
            {
                var start = connectionString.IndexOf("=", prestart) + 1;
                var nextIdx = connectionString.IndexOf(";", start);
                if (nextIdx > -1)  // mid connstr e.g. ";password=1234567;next=element"
                {
                    return connectionString.Substring(0, start) +
                        Decrypt.String(connectionString.Substring(start, nextIdx - start), options: cryptoOptions) +
                        connectionString.Substring(nextIdx);
                }
                else  // connstr end
                {
                    return connectionString.Substring(0, start) +
                        Decrypt.String(connectionString.Substring(start), options: cryptoOptions);
                }
            }
            return connectionString;
        }

        ///// <summary>
        ///// Utility method for inline decrypting the password in a connection string
        ///// </summary>
        ///// <param name="connectionString">A connection string potentially with an encrypted password</param>
        ///// <param name="cryptoOptions">Optional cryptographic properties used to decrypt database passwords</param>
        ///// <returns>A sensitive connection string i.e. may contain user id and password.</returns>
        //public static string DecryptInlinePassword(string connectionString, CryptoOptions cryptoOptions = null)
        //{
        //    var cipherText = ParseConnectionStringValue(ConnectionStringPart.Password, connectionString);
        //    if (cipherText == null)
        //        return connectionString;
        //    var plainText = Decrypt.String(cipherText, options: cryptoOptions);
        //    var sensitiveConnectionString = connectionString.Replace(cipherText, plainText);
        //    return sensitiveConnectionString;
        //}

        /// <summary>
        /// Utility method for inline encrypting the password in a connection string
        /// </summary>
        /// <param name="connectionString">A connection string potentially with an encrypted password</param>
        /// <returns>A version of the connection string with the password redatcted.</returns>
        public static string HideInlinePassword(string connectionString)
        {
            if (connectionString == null)
                return string.Empty;
            var prestart = Math.Max
            (
                connectionString.IndexOf(";password=", StringComparison.OrdinalIgnoreCase),
                connectionString.IndexOf(";pwd=", StringComparison.OrdinalIgnoreCase)
            );
            if (prestart > -1)
            {
                var start = connectionString.IndexOf("=", prestart) + 1;
                var nextIdx = connectionString.IndexOf(";", start);
                if (nextIdx > -1)  // mid connstr e.g. ";password=1234567;next=element"
                {
                    return connectionString.Substring(0, start) +
                        new string('*', nextIdx - start) +
                        connectionString.Substring(nextIdx);
                }
                else  // connstr end
                {
                    return connectionString.Substring(0, start) + new string('*', connectionString.Length - start);
                }
            }
            return connectionString;
        }

        ///// <summary>
        ///// Utility method for inline encrypting the password in a connection string (another version)
        ///// </summary>
        ///// <param name="connectionString">A connection string potentially with an encrypted password</param>
        ///// <returns>A version of the connection string with the password redatcted.</returns>
        //public static string HideInlinePassword(string connectionString)
        //{
        //    var passwordText = ParseConnectionStringValue(ConnectionStringPart.Password, connectionString);
        //    if (passwordText == null) 
        //        return connectionString;
        //    var safeConnectionString = connectionString.Replace(passwordText, new string('*', passwordText.Length));
        //    return safeConnectionString;
        //}

        /// <summary>
        /// Extract a connection string element's value
        /// </summary>
        /// <param name="key">The connection string element's key (e.g. "Data Source", "Initial Catalog", etc.)</param>
        /// <param name="connectionString">a connection string</param>
        /// <returns></returns>
        public static string ParseConnectionStringValue(string key, string connectionString)
        {
            var match = new Regex("(?<=" + key + "=)[^;]+", RegexOptions.IgnoreCase).Match(connectionString);
            return Zap.String(match.Value);
        }

        /// <summary>
        /// Extract a connection string element's value
        /// </summary>
        /// <param name="part">Analagous to the key in <see cref="ParseConnectionStringValue(string,string)"/></param>
        /// <param name="connectionString">a connection string</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
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
            throw new ThisShouldNeverHappenException("Unknown connection string part: " + part); 
        }

        /// <summary>
        /// Parse additional connection attributes which are stored pipe delimited (|) in configuration
        /// </summary>
        /// <param name="text">the pipe delimited text value from configuration</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
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
                    throw new ValidationException("Invalid additional connection string attribute: " + attr);
                }
            }
            return dict;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *         TABLES AND COLUMNS          *
         * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// When generating the SQL for parameters it may be 
        /// necessary to 'fix' a column name in C# to be valid in SQL, e.g. adding quotes or square brackets 
        /// around the column name especially if it contains spaces or other non-word characters.
        /// </summary>
        /// <param name="parameter">a parameter</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public static string RenderColumnName(DbParameter parameter, DbProvider provider = default)
        {
            if (Zap.String(parameter.ParameterName) == null)
                throw new ValidationException("column name cannot be null");
            return RenderColumnName(parameter.ParameterName, provider: provider);
        }

        /// <summary>
        /// When generating the SQL for parameters or in other situations it may be 
        /// necessary to 'fix' a column name in C# to be valid in SQL, e.g. adding quotes or square brackets 
        /// around the column name especially if it contains spaces or other non-word characters.
        /// </summary>
        /// <param name="columnName">a column name</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        public static string RenderColumnName(string columnName, DbProvider provider = default)
        {
            return DbUtilAbstractions.RenderColumnName(columnName, provider: provider);
        }

        /// <summary>
        /// Iterates through a data table and trims each <c>string</c>
        /// </summary>
        /// <param name="dataTable">a data table</param>
        /// <exception cref="ValidationException"></exception>
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
                    throw new ValidationException("Row items do not match field types: " + dataRow.ItemArray.Length + ", " + fieldTypes.Length);
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

        /// <summary>
        /// Gets all field values from the current row of an open <c>IDataReader</c> as an <c>object[]</c>.
        /// </summary>
        /// <param name="reader">An open DB data reader</param>
        /// <param name="autoTrunc">how to handle strings</param>
        /// <returns></returns>
        public static object[] GetAllRowValues(IDataReader reader, AutoTruncate autoTrunc = default)
        {
            var items = new object[reader.FieldCount];

            for (int i = 0; i < items.Length; i++)
            {
                if (reader.IsDBNull(i))
                    continue;
                items[i] = Zap.Object(reader[i]);
                if (items[i] is string stringValue)
                {
                    if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    {
                        if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                        {
                            if (string.IsNullOrWhiteSpace(stringValue))
                            {
                                if (stringValue.Length == 0)
                                    items[i] = null;
                            }
                            else
                                items[i] = stringValue.Trim();
                        }
                        else
                        {
                            items[i] = stringValue = stringValue.Trim();
                            if (stringValue.Length == 0)
                                items[i] = null;
                        }
                    }
                    else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                        items[i] = stringValue.Trim();
                }
            }
            return items;
        }

        /// <summary>
        /// Gets all field values from the current row of an open <c>IDataReader</c> as an <c>object[]</c>.
        /// </summary>
        /// <param name="reader">An open DB data reader</param>
        /// <param name="autoTrunc">how to handle strings</param>
        /// <returns></returns>
        public static async Task<object[]> GetAllRowValuesAsync(IDataReader reader, AutoTruncate autoTrunc = default)
        {
            var items = new object[reader.FieldCount];

            for (int i = 0; i < items.Length; i++)
            {
                if (reader is DbDataReader dbDataReader && await dbDataReader.IsDBNullAsync(i))
                    continue;
                items[i] = Zap.Object(reader[i]);
                if (items[i] is string stringValue)
                {
                    if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    {
                        if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                        {
                            if (string.IsNullOrWhiteSpace(stringValue))
                            {
                                if (stringValue.Length == 0)
                                    items[i] = null;
                            }
                            else
                                items[i] = stringValue.Trim();
                        }
                        else
                        {
                            items[i] = stringValue = stringValue.Trim();
                            if (stringValue.Length == 0)
                                items[i] = null;
                        }
                    }
                    else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                        items[i] = stringValue.Trim();
                }
            }
            return items;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">An oben DB data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IEnumerable<object[]> ReadAsObjects(DbDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ReadAsObjects(reader)");
            journal.Level++;

            // data stuff
            var list = new List<object[]>();
            var cols = reader.GetDataColumns();
            if (dbCapture != null)
                dbCapture.DataColumns = cols;
            while (reader.Read())
            {
                var objects = GetAllRowValues(reader, autoTrunc: autoTrunc);
                list.Add(objects);
            }

            // finalize
            journal.Level--;
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">An oben DB data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IEnumerable<object[]>> ReadAsObjectsAsync(DbDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ReadAsObjectsAsync(reader)");
            journal.Level++;

            // data stuff
            var list = new List<object[]>();
            if (dbCapture != null)
                dbCapture.DataColumns = reader.GetDataColumns();
            while (await reader.ReadAsync())
            {
                var objects = await GetAllRowValuesAsync(reader, autoTrunc: autoTrunc);
                list.Add(objects);
            }

            // finalize
            journal.Level--;
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IEnumerable<object[]> ReadAsObjects(DbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ReadAsObjects(command)");
            journal.Level++;

            // data stuff
            using (var reader = command.ExecuteReader())
            {
                var list = ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                if (dbCapture != null)
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();

                // finalize
                journal.Level--;
                return list;
            }
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IEnumerable<object[]>> ReadAsObjectsAsync(DbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ReadAsObjectsAsync(command)");
            journal.Level++;

            // data stuff
            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = await ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                if (dbCapture != null)
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();

                // finalize
                journal.Level--;
                return list;
            }
        }

        /// <summary>
        /// Reads data from a reader and returns parsed instances of <c>T</c>. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="command">An open DB command.</param>
        /// <param name="readerParser">A parser that reads directly from an open data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Parsed instances of <c>T</c>.</returns>
        public static IEnumerable<T> ParseRows<T>(DbCommand command, Func<IDataReader, T> readerParser, DbCapture dbCapture = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ParseRows(command)");
            journal.Level++;

            // data stuff
            var list = new List<T>();
            using (var reader = command.ExecuteReader())
            {
                if (dbCapture != null)
                {
                    dbCapture.DataColumns = reader.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }
                while (reader.Read())
                {
                    list.Add(readerParser.Invoke(reader));
                }
            }

            // finalize
            journal.Level--;
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns parsed instances of <c>T</c>. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="command">An open DB command.</param>
        /// <param name="readerParser">A parser that reads directly from an open data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>Parsed instances of <c>T</c>.</returns>
        public static async Task<IEnumerable<T>> ParseRowsAsync<T>(DbCommand command, Func<IDataReader, T> readerParser, DbCapture dbCapture = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.ParseRowsAsync(command)");
            journal.Level++;

            // data stuff
            var list = new List<T>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (dbCapture != null)
                {
                    dbCapture.DataColumns = reader.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }
                while (await reader.ReadAsync())
                {
                    list.Add(readerParser.Invoke(reader));
                }
            }

            // finalize
            journal.Level--;
            return list;
        }

        /// <summary>
        /// Prepare an object for insertion into a SQL statement.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        public static string Sqlize(object obj, DbProvider provider = default)
        {
            return DbUtilAbstractions.Sqlize(obj, provider: provider);
        }

        /// <summary>
        /// Displays the contents of a data table in <c>string</c> form.
        /// </summary>
        /// <param name="dataTable">A <c>DataTable</c>.</param>
        /// <returns>A text representation of a <c>DataTable</c>.</returns>
        public static string Dump(DataTable dataTable)
        {
            var sb = new StringBuilder();
            var colWidths = new int[dataTable.Columns.Count];
            int _width;

            // prep widths - column names
            for (int i = 0; i < colWidths.Length; i++)
            {
                colWidths[i] = dataTable.Columns[i].ColumnName.Length;
            }

            // prep widths - data values
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < colWidths.Length; i++)
                {
                    _width = TextUtil.DumpDatum(row[i]).Length;
                    if (_width > colWidths[i])
                    {
                        colWidths[i] = _width;
                    }
                }
            }

            // build column headers
            for (int i = 0; i < colWidths.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(dataTable.Columns[i].ColumnName.PadRight(colWidths[i]));
            }
            sb.AppendLine();

            // build separators
            for (int i = 0; i < colWidths.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append("".PadRight(colWidths[i], '-'));
            }
            sb.AppendLine();

            // build data rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(TextUtil.DumpDatum(row[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds a statement to delete one or more rows from a table or drop (truncate) the table based on the default provider.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="drop">If <c>true</c>, returns a statement to delete the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, returns a statement to delete the table database object (rather than just delete rows) and release the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildDeleteStatement(string tableName, IFilter where, bool drop = false, bool purge = false, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildDeleteStatement(\"" + tableName + "\")");
            journal.Level++;

            // data stuff
            try
            {
                return BuildDeleteStatement(DbSettings.DefaultProvider, tableName, where, drop: drop, purge: purge, journal: journal);
            }
            finally
            {
                //finalize
                journal.Level--;
            }
        }

        /// <summary>
        /// Builds a provider-specific statement to delete one or more rows from a table or drop (truncate) the table.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="drop">If <c>true</c>, returns a statement to delete the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, returns a statement to delete the table database object (rather than just delete rows) and release the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildDeleteStatement(DbProvider provider, string tableName, IFilter where, bool drop = false, bool purge = false, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildDeleteStatement(" + provider + ", \"" + tableName + "\")");
            journal.Level++;

            // data stuff
            string statement;
            journal.WriteEntry("provider = " + provider);

            switch (provider)
            {
                case DbProvider.SqlServer:
                    if (drop)
                    {
                        statement = "TRUNCATE TABLE " + tableName;
                    }
                    else
                    {
                        statement = "DELETE FROM " + tableName;
                        if (where != null)
                            statement += " WHERE " + where.Render(DbProvider.SqlServer);
                    }
                    break;
                case DbProvider.Oracle:
                    if (drop)
                    {
                        statement = "DROP TABLE " + tableName;
                        if (purge)
                            statement += " PURGE";
                    }
                    else
                    {
                        statement = "DELETE FROM " + tableName;
                        if (where != null)
                            statement += " WHERE " + where.Render(DbProvider.Oracle);
                    }
                    statement += ";";
                    break;
                default:
                    throw new ArgumentException("This method is currently compatible only with certain providers: SqlServer, Oracle");
            }

            journal.AddAndWriteEntry("sql.statement", statement);

            //finalize
            journal.Level--;
            return statement;
        }

        /// <summary>
        /// Builds a statement to insert a row into a table based on the default provider.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildInsertStatement(string tableName, IEnumerable<DbParameter> columns, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildInsertStatement(\"" + tableName + "\")");
            journal.Level++;

            // data stuff
            try
            {
                return BuildInsertStatement(DbSettings.DefaultProvider, tableName, columns, journal: journal);
            }
            finally
            {
                //finalize
                journal.Level--;
            }
        }

        /// <summary>
        /// Builds a provider-specific statement to insert a row into a table.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildInsertStatement(DbProvider provider, string tableName, IEnumerable<DbParameter> columns, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildInsertStatement(" + provider + ", \"" + tableName + "\")");
            journal.Level++;

            // data stuff
            string statement;
            journal.WriteEntry("provider = " + provider);

            switch (provider)
            {
                case DbProvider.SqlServer:
                    statement = "INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => RenderColumnName(c, provider: DbProvider.SqlServer))) + ")";
                    statement += " VALUES (" + string.Join(", ", columns.Select(c => Sqlize(c.Value, provider: DbProvider.SqlServer))) + ")";
                    break;
                case DbProvider.Oracle:
                    statement = "INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => RenderColumnName(c, provider: DbProvider.Oracle))) + ")";
                    statement += " VALUES (" + string.Join(", ", columns.Select(c => Sqlize(c.Value, provider: DbProvider.Oracle))) + ");";
                    break;
                default:
                    throw new ArgumentException("This method is currently compatible only with certain providers: SqlServer, Oracle");
            }

            journal.AddAndWriteEntry("sql.statement", statement);

            //finalize
            journal.Level--;
            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to insert a row into a table and get the resulting identity.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="getIdentitySql">An optional SELECT statement for retrieving the identity of the inserted row.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildInsertAndGetIdentityStatements(string tableName, IEnumerable<DbParameter> columns, string getIdentitySql = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildInsertAndGetIdentityStatements(\"" + tableName + "\")");
            journal.Level++;

            // data stuff
            try
            {
                return BuildInsertAndGetIdentityStatements(DbSettings.DefaultProvider, tableName, columns, getIdentitySql: getIdentitySql, journal: journal);
            }
            finally
            {
                //finalize
                journal.Level--;
            }
        }

        /// <summary>
        /// Builds a provider-specific statement to insert a row into a table and get the resulting identity.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="getIdentitySql">An optional SELECT statement for retrieving the identity of the inserted row.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildInsertAndGetIdentityStatements(DbProvider provider, string tableName, IEnumerable<DbParameter> columns, string getIdentitySql = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildInsertAndGetIdentityStatements(" + provider + ", \"" + tableName + "\")");
            journal.Level++;

            // data stuff
            string statement = BuildInsertStatement(provider, tableName, columns, journal: journal);

            if (getIdentitySql != null)
            {
                statement += " " + getIdentitySql;
            }
            else
            {
                switch (provider)
                {
                    case DbProvider.SqlServer:
                        statement += " SELECT " + SqlLiteral.Identity(DbProvider.SqlServer);
                        break;
                    case DbProvider.Oracle:
                        statement += " SELECT " + SqlLiteral.Identity(DbProvider.Oracle) + ";";
                        break;
                    default:
                        throw new ArgumentException("This method is currently compatible only with certain providers: SqlServer, Oracle");
                }
            }

            journal.AddAndWriteEntry("sql.statement", statement);

            //finalize
            journal.Level--;
            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to update one or more rows in a table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildUpdateStatement(string tableName, IEnumerable<DbParameter> columns, IFilter where, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildUpdateStatement(\"" + tableName + "\")");
            journal.Level++;

            // data stuff
            try
            {
                return BuildUpdateStatement(DbSettings.DefaultProvider, tableName, columns, where, journal: journal);
            }
            finally
            {
                //finalize
                journal.Level--;
            }
        }

        /// <summary>
        /// Builds a provider-specific statement to update one or more rows in a table.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildUpdateStatement(DbProvider provider, string tableName, IEnumerable<DbParameter> columns, IFilter where, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildUpdateStatement(" + provider + ", \"" + tableName + "\")");
            journal.Level++;

            // data stuff
            string statement;
            journal.WriteEntry("provider = " + provider);

            switch (provider)
            {
                case DbProvider.SqlServer:
                    statement = "UPDATE " + tableName + "SET " + string.Join(", ", columns.Select(c => c.ToDMLString(provider: DbProvider.SqlServer)));
                    if (where != null)
                        statement += " WHERE " + where.Render(DbProvider.SqlServer);
                    break;
                case DbProvider.Oracle:
                    statement = "UPDATE " + tableName + "SET " + string.Join(", ", columns.Select(c => c.ToDMLString(provider: DbProvider.Oracle)));
                    if (where != null)
                        statement += " WHERE " + where.Render(DbProvider.Oracle);
                    statement += ";";
                    break;
                default:
                    throw new ArgumentException("This method requires a non-neutral DB provider.");
            }

            journal.AddAndWriteEntry("sql.statement", statement);

            //finalize
            journal.Level--;
            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to call a function.
        /// </summary>
        /// <param name="functionName">A function name.</param>
        /// <param name="parameters">Optional parameter(s) to pass into the function.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildFunctionStatement(string functionName, IEnumerable<DbParameter> parameters = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildFunctionStatement(\"" + functionName + "\")");
            journal.Level++;

            try
            {
                return BuildFunctionStatement(DbSettings.DefaultProvider, functionName, parameters: parameters, journal: journal);
            }
            finally
            {
                //finalize
                journal.Level--;
            }
        }

        /// <summary>
        /// Builds a provider-specific statement to call a function.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="functionName">A function name.</param>
        /// <param name="parameters">Optional parameter(s) to pass into the function.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The SQL statement.</returns>
        public static string BuildFunctionStatement(DbProvider provider, string functionName, IEnumerable<DbParameter> parameters = null, TraceJournal journal = null)
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("DbUtil.BuildFunctionStatement(" + provider + ", \"" + functionName + "\")");
            journal.Level++;

            // data stuff
            string statement;
            journal.WriteEntry("provider = " + provider);

            switch (provider)
            {
                case DbProvider.SqlServer:
                    if (parameters == null)
                    {
                        statement = "SELECT * FROM " + functionName + "()";
                    }
                    else
                    {
                        statement = "SELECT * FROM " + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, provider: DbProvider.SqlServer))) + ")";
                    }
                    break;
                case DbProvider.Oracle:
                    if (parameters == null)
                    {
                        statement = "SELECT * FROM TABLE(" + functionName + "());";
                    }
                    else
                    {
                        statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, provider: DbProvider.Oracle))) + "));";
                    }
                    break;
                default:
                    throw new ArgumentException("This method is currently compatible only with certain providers: SqlServer, Oracle");
            }

            journal.AddAndWriteEntry("sql.statement", statement);

            //finalize
            journal.Level--;
            return statement;
        }

        /// <summary>
        /// Use this method to build a <c>Credential</c> dynamically and for any use.
        /// </summary>
        /// <param name="userId">A user name or ID</param>
        /// <param name="password">A password</param>
        /// <param name="isEncryptedPassword">If <c>true</c>, instructs the system to decrypt the password (if supplied).  Default is <c>false</c>.</param>
        /// <param name="cryptoOptions">Optional symmetric algorithm properties used to decrypt passwords</param>
        /// <returns>A <c>Credential</c></returns>
        public static Credential? BuildCredentials(string userId, string password, bool isEncryptedPassword = false, CryptoOptions cryptoOptions = null)
        {
            if (userId == null)
                return null;
            if (password != null && isEncryptedPassword)
                return new Credential(userId, Decrypt.SecureString(password, options: cryptoOptions));
            return Credential.Build(userId, password);
        }

        /// <summary>
        /// Applies <c>AutoTruncate</c> on scalar query results replacing <c>DBNull</c> with <c>null</c>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="autoTrunc"></param>
        /// <returns></returns>
        public static object ProcessScalarResult(object result, AutoTruncate autoTrunc = default)
        {
            if (ObjectUtil.IsNull(result))
                return null;
            if (result is string stringValue)
            {
                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                {
                    if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            if (stringValue.Length == 0)
                                return null;
                        }
                        else
                            stringValue = stringValue.Trim();
                    }
                    else
                    {
                        stringValue = stringValue.Trim();
                        if (stringValue.Length == 0)
                            return null;
                    }
                }
                else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    stringValue = stringValue.Trim();
                return stringValue;
            }
            return result;
        }
    }
}
