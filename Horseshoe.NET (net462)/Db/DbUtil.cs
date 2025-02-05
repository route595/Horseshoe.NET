using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Configuration;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A collection of common, provider agnostic factory methods for Horseshoe.NET DB operations.
    /// </summary>
    public static class DbUtil
    {
        private static readonly string MessageRelayGroup = typeof(DbUtil).Namespace;

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
        //
        ///// <returns>A new <c>ConnectionInfo</c> object whose <c>ConnectionString</c> should be used to build connections.</returns>
        ///// <exception cref="ValidationException"></exception>
        //public static T LoadConnectionInfo<T>(T connectionInfo, Func<string> buildConnectionStringFromConfig, CryptoOptions cryptoOptions = null, Func<string> getConnectionStringSource = null) where T : ConnectionInfo, new()
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
        /// can listen to <c>SystemMessageRelay</c> to get details including the connection string source.
        /// </summary>
        /// <param name="connectionInfo">The <c>ConnectionInfo</c> supplied to the database method, can be <c>null</c></param>
        /// <param name="buildConnectionStringFromConfig">A provider specific function for building a connection string from configuration file</param>
        /// <param name="cryptoOptions">Optional options for the crypto engine to decrypt the connection string password. Source should be DB settings.</param>
        /// <returns>A new <c>ConnectionInfo</c> object whose <c>ConnectionString</c> should be used to build connections.</returns>
        /// <exception cref="DbException"></exception>
        public static T LoadFinalConnectionInfo<T>(T connectionInfo, Func<string> buildConnectionStringFromConfig, CryptoOptions cryptoOptions = null) where T : ConnectionInfo, new()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

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
                else if (connectionInfo.ConnectionStringName is string connStrName && !string.IsNullOrEmpty(connStrName) && Config.GetConnectionString(connStrName) is string _connStrN && !string.IsNullOrEmpty(_connStrN))
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

                // decrypt connection string password
                if (connectionInfo.IsEncryptedPassword)
                    connStr = DecryptInlinePassword(connStr, cryptoOptions: cryptoOptions ?? connectionInfo.CryptoOptions);

                if (connStr != null)
                {
                    var finalConnectionInfo = new T
                    {
                        ConnectionString = connStr,
                        Credentials = connectionInfo.Credentials
                    };
                    SystemMessageRelay.RelayMessage("Connection string: " + HideInlinePassword(connStr), group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_CONNECTION_STRING);
                    SystemMessageRelay.RelayMessage("Connection string source: " + connStrSource, group: MessageRelayGroup, id: DbConstants.MessageRelay.CONNECTION_STRING_SOURCE);
                    SystemMessageRelay.RelayMessage("Credentials: " + TextUtil.Reveal(finalConnectionInfo.Credentials?.UserName), group: MessageRelayGroup);
                    SystemMessageRelay.RelayMethodReturn(returnDescription: nameof(finalConnectionInfo), group: MessageRelayGroup);
                    return finalConnectionInfo;
                }
            }

            // finally, check config

            // check config connection string
            if (Config.Get("Horseshoe.NET:" + configPart + ":ConnectionString") is string _connStrC && !string.IsNullOrEmpty(_connStrC))
            {
                connStr = _connStrC;
                connStrSource = "config-connection-string";
            }
            // then check config connection string name
            else if (Config.Get("Horseshoe.NET:" + configPart + ":ConnectionStringName") is string connStrName && !string.IsNullOrEmpty(connStrName) && Config.GetConnectionString(connStrName) is string _connStrN && !string.IsNullOrEmpty(_connStrN))
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

            // decrypt connection string password
            if (Config.Get<bool>("Horseshoe.NET:" + configPart + ":IsEncryptedPassword"))
                connStr = DecryptInlinePassword(connStr, cryptoOptions: cryptoOptions);

            if (connStr != null)
            {
                var finalConnectionInfo = new T
                {
                    ConnectionString = connStr,
                    Credentials = BuildCredentials(Config.Get("Horseshoe.NET:" + configPart + ":UserID"),
                                                   Config.Get("Horseshoe.NET:" + configPart + ":Password"),
                                                   Config.Get<bool>("Horseshoe.NET:" + configPart + ":IsEncryptedPassword"),
                                                   cryptoOptions: cryptoOptions)
                };
                SystemMessageRelay.RelayMessage("Connection string: " + HideInlinePassword(connStr), group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_CONNECTION_STRING);
                SystemMessageRelay.RelayMessage("Connection string source: " + connStrSource, group: MessageRelayGroup, id: DbConstants.MessageRelay.CONNECTION_STRING_SOURCE);
                SystemMessageRelay.RelayMessage("Credentials: " + TextUtil.Reveal(finalConnectionInfo.Credentials?.UserName), group: MessageRelayGroup);
                SystemMessageRelay.RelayMethodReturn(returnDescription: nameof(finalConnectionInfo), group: MessageRelayGroup);
                //revealConnectionStringSource?.Invoke(connStrSource);
                return finalConnectionInfo;
            }

            var ex = new DbException(string.Concat("No connection info could be found", Assemblies.Find("Horseshoe.NET.Configuration") == null ? " perhaps due to Horseshoe.NET.Configuration is not installed" : "", "."));
            SystemMessageRelay.RelayException(ex);
            throw ex;
        }

        /// <summary>
        /// Utility method for inline decrypting the password in a connection string
        /// </summary>
        /// <param name="connectionString">A connection string potentially with an encrypted password</param>
        /// <param name="cryptoOptions">Optional cryptographic properties used in the decryption process</param>
        /// <param name="altPasswordAttributeNameVariations">Optionally override the attribute name variations searched for in the connection string e.g. "Password" or new[] { "Password", "PWD" }.  Default is the example array.</param>
        /// <param name="ignoreCase">If <c>true</c> (recommended), ignores the title case when searching for <c>altPasswordAttributeNameVariations</c>, default is <c>false</c>.</param>
        /// <returns>A sensitive connection string i.e. may contain user id and password.</returns>
        public static string DecryptInlinePassword(string connectionString, CryptoOptions cryptoOptions = null, StringValues? altPasswordAttributeNameVariations = null, bool ignoreCase = false)
        {
            ParseConnectionStringAttribute(connectionString, altPasswordAttributeNameVariations ?? new[] { "Password", "PWD" }, out int attributeValuePosition, out string attributeValue, ignoreCase: !altPasswordAttributeNameVariations.HasValue || ignoreCase);
            
            if (attributeValuePosition == -1)
                return connectionString ?? string.Empty;

            if (connectionString.Length > attributeValuePosition + attributeValue.Length)  // mid connstr e.g. "...;password=123456;next=element..."
            {
                return connectionString.Substring(0, attributeValuePosition) +
                    Decrypt.String(attributeValue.Trim(), options: cryptoOptions) +
                    connectionString.Substring(attributeValuePosition + attributeValue.Length);
            }
            else  // connstr end e.g. "...;password=123456"
            {
                return connectionString.Substring(0, attributeValuePosition) +
                    Decrypt.String(attributeValue.Trim(), options: cryptoOptions);
            }
        }

        /// <summary>
        /// Utility method for inline encrypting the password in a connection string
        /// </summary>
        /// <param name="connectionString">A connection string potentially with an encrypted password</param>
        /// <param name="altPasswordAttributeNameVariations">Optionally override the attribute name variations searched for in the connection string e.g. "Password" or new[] { "Password", "PWD" }.  Default is the example array.</param>
        /// <param name="ignoreCase">If <c>true</c> (recommended), ignores the title case when searching for <c>altPasswordAttributeNameVariations</c>, default is <c>false</c>.</param>
        /// <returns>A version of the connection string with the password redatcted.</returns>
        public static string HideInlinePassword(string connectionString, StringValues? altPasswordAttributeNameVariations = null, bool ignoreCase = false)
        {
            ParseConnectionStringAttribute(connectionString, altPasswordAttributeNameVariations ?? new[] { "Password", "PWD" }, out int attributeValuePosition, out string attributeValue, ignoreCase: !altPasswordAttributeNameVariations.HasValue || ignoreCase);

            if (attributeValuePosition == -1)
                return connectionString ?? string.Empty;

            if (connectionString.Length > attributeValuePosition + attributeValue.Length)  // mid connstr e.g. "...;password=123456;next=element..."
            {
                return connectionString.Substring(0, attributeValuePosition) +
                    new string('*', attributeValue.Trim().Length) +
                    connectionString.Substring(attributeValuePosition + attributeValue.Length);
            }
            else  // connstr end e.g. "...;password=123456"
            {
                return connectionString.Substring(0, attributeValuePosition) +
                    new string('*', attributeValue.Trim().Length);
            }
        }

        /// <summary>
        /// Pass in a connection string and the desired attribute name variation(s).
        /// Method returns (via out parameters) the attribute value and its position in the connection string.
        /// <example>
        /// <para>
        /// Example 1
        /// </para>
        /// <code>
        /// connStr = "Datasource=MyDB;UID=Bob;xPWD=th3;PWD=Bui1d3r";
        /// attributeNameVariations = new[] { "Password", "PWD" };
        /// ParseConnectionStringAttribute(connStr, attributeNameVariations, out int attributeValuePosition, out string attributeValue, ignoreCase: true);
        /// </code>
        /// <para>
        /// Steps:
        /// 1 - connection string searched for occurrences of "Password", -1 (not found)
        /// 2 - connection string searched for occurrences of "PWD"...
        /// 3 - match found in "xPWD=th3"; then rejected (set to -1) when the "x" is scanned
        /// 4 - match found in "PWD=Bui1d3r"; accepted
        /// 5 - method returns 37, "Bui1d3r"
        /// </para>
        /// </example>
        /// </summary>
        /// <param name="connectionString">A connection string</param>
        /// <param name="attributeNameVariations">One or more attribue name variations (e.g. "ConnectionTimeout" or new[] { "Password", "PWD" })</param>
        /// <param name="attributeValuePosition">The matching attribute's value's position in the connection string is returned here.</param>
        /// <param name="attributeValue">The matching attribute's untrimmed value is returned here.</param>
        /// <param name="ignoreCase">If <c>true</c> (recommended), ignores the title case when searching for attribute name variations, default is <c>false</c>.</param>
        /// <exception cref="DbException"></exception>
        public static void ParseConnectionStringAttribute(string connectionString, StringValues attributeNameVariations, out int attributeValuePosition, out string attributeValue, bool ignoreCase = false)
        {
            if (connectionString == null)
            {
                attributeValuePosition = -1;
                attributeValue = null;
                return;
            }

            if (!attributeNameVariations.Any())
                throw new DbException("No connection string attribute name variations were supplied");

            if (attributeNameVariations.Any(str => string.IsNullOrWhiteSpace(str)))
                throw new DbException("Connection string attribute name variations cannot be null or blank");

            int anvPos = -1;  // attribute name variation position
            char c;

            foreach (var attributeNameVariation in attributeNameVariations)
            {
                // get position of attribute name variation, in the while loop this position will either be accepted or rejected
                anvPos = connectionString.IndexOf(attributeNameVariation + "=", ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

                while (anvPos > -1)
                {
                    // scan previous chars until ';' or beginning of string is reached
                    for (int j = anvPos - 1; j >= 0; j--) // simply won't loop if -1 (not found); simple and efficient algorithm
                    {
                        c = connectionString[j];
                        if (c == ' ')           // ignore if space (' ') and continue scanning
                            continue;
                        if (c != ';')           // char found that is not space (' ') or semicolon (';')
                            anvPos = -1;        // reject attribute name variation
                        break;                  // in either case stop scanning
                    }

                    // scan next chars until '=' or end of string is reached
                    if (anvPos > -1)
                    {
                        for (int j = anvPos + attributeNameVariation.Length; j < connectionString.Length; j++)
                        {
                            c = connectionString[j];
                            if (c == ' ')       // ignore if space (' ') and continue scanning
                                continue;
                            if (c != '=')       // char found that is not space (' ') or equals sign ('=')
                                anvPos = -1;    // reject attribute name variation
                            break;              // in either case stop scanning
                        }
                    }

                    if (anvPos > -1)            // attribute name variation accepted
                        break;                  // exit inner loop a.k.a. skip any other occurrences of current variation

                    // attribute name variation rejected, search for next occurrence
                    anvPos = connectionString.IndexOf(attributeNameVariation + "=", anvPos, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                }
                if (anvPos > -1)                // attribute name variation found
                    break;                      // exit inner loop a.k.a. skip any other variations
            }

            if (anvPos == -1)                   // attribute name variation(s) not found
            {
                attributeValuePosition = -1;
                attributeValue = null;
                return;
            }

            attributeValuePosition = connectionString.IndexOf("=", anvPos) + 1;
            var attributeValueEndPos = connectionString.IndexOf(";", attributeValuePosition);  // it doesn't matter if ';' not found and valueEndPos == -2
            if (attributeValueEndPos > 0)  // mid connstr e.g. "...;attribute=VALUE;nextAttribute=nextValue..."
            {
                attributeValue = connectionString.Substring(attributeValuePosition, attributeValueEndPos - attributeValuePosition);
            }
            else  // connstr end "...;attribute=VALUE"
            {
                attributeValue = connectionString.Substring(attributeValuePosition);
            }
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
        /// <exception cref="DbException"></exception>
        public static IDictionary<string, string> ParseAdditionalConnectionAttributes(string text)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(text))
                return dict;
            var list = Zap.Strings(text.Split('|'), pruneOptions: PruneOptions.All);
            foreach (var attr in list)
            {
                var attrParts = attr.Split('=').TrimAll();
                if (attrParts.Length == 2)
                {
                    dict.Add(attrParts[0], attrParts[1]);
                }
                else
                {
                    throw new DbException("Invalid additional connection string attribute: " + attr);
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
        /// <exception cref="DbException"></exception>
        public static string RenderColumnName(DbParameter parameter, DbProvider provider = default)
        {
            if (Zap.String(parameter.ParameterName) == null)
                throw new DbException("column name cannot be null");
            return RenderColumnName(parameter.ParameterName, provider: provider);
        }

        /// <summary>
        /// Iterates through a data table and trims each <c>string</c>
        /// </summary>
        /// <param name="dataTable">a data table</param>
        /// <exception cref="DbException"></exception>
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
                    throw new DbException("Row items do not match field types: " + dataRow.ItemArray.Length + ", " + fieldTypes.Length);
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
        
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IList<object[]> ReadAsObjects(IDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<object[]>();
            var cols = reader.GetDataColumns();
            if (dbCapture != null)
                dbCapture.DataColumns = cols;
            while (reader.Read())
            {
                var objects = GetAllRowValues(reader, autoTrunc: autoTrunc);
                list.Add(objects);
            }

            SystemMessageRelay.RelayMethodReturn(returnDescription: cols.Length + " cols x " + list.Count + " rows", group: MessageRelayGroup);
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">An oben DB data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IList<object[]>> ReadAsObjectsAsync(IDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<object[]>();
            var cols = reader.GetDataColumns();
            if (dbCapture != null)
                dbCapture.DataColumns = cols;
            if (reader is DbDataReader dbDataReader)
            {
                while (await dbDataReader.ReadAsync())
                {
                    var objects = await GetAllRowValuesAsync(reader, autoTrunc: autoTrunc);
                    list.Add(objects);
                }
            }
            else
            {
                while (reader.Read())
                {
                    var objects = await GetAllRowValuesAsync(reader, autoTrunc: autoTrunc);
                    list.Add(objects);
                }
            }

            SystemMessageRelay.RelayMethodReturn(returnDescription: cols.Length + " cols x " + list.Count + " rows", group: MessageRelayGroup);
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IList<object[]> ReadAsObjects(DbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var reader = command.ExecuteReader())
            {
                var list = ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);
                var outParams = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();
                SystemMessageRelay.RelayMessage("out params: " + (outParams.Length != 0 ? string.Join(", " + outParams.Select(p => p.ParameterName + " = " + Sqlize(p.Value))) : "none"), group: MessageRelayGroup);
                if (dbCapture != null)
                    dbCapture.OutputParameters = outParams;

                SystemMessageRelay.RelayMethodReturn(returnDescription: list.Count() + " rows", group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IList<object[]>> ReadAsObjectsAsync(DbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var reader = await command.ExecuteReaderAsync())
            {
                var list = await ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);
                var outParams = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();
                SystemMessageRelay.RelayMessage("out params: " + (outParams.Length != 0 ? string.Join(", " + outParams.Select(p => p.ParameterName + " = " + Sqlize(p.Value))) : "none"), group: MessageRelayGroup);
                if (dbCapture != null)
                    dbCapture.OutputParameters = outParams;

                SystemMessageRelay.RelayMethodReturn(returnDescription: "read " + list.Count() + " rows", group: MessageRelayGroup);
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
        /// <returns>Parsed instances of <c>T</c>.</returns>
        public static IList<T> ParseRows<T>(DbCommand command, Func<IDataReader, T> readerParser, DbCapture dbCapture = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<T>();
            using (var reader = command.ExecuteReader())
            {
                var cols = reader.GetDataColumns();
                var outParams = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();

                SystemMessageRelay.RelayMessage("out params: " + (outParams.Length != 0 ? string.Join(", " + outParams.Select(p => p.ParameterName + " = " + Sqlize(p.Value))) : "none"), group: MessageRelayGroup);
                
                if (dbCapture != null)
                {
                    dbCapture.DataColumns = cols;
                    dbCapture.OutputParameters = outParams;
                }
                while (reader.Read())
                {
                    list.Add(readerParser.Invoke(reader));
                }

                SystemMessageRelay.RelayMethodReturn(returnDescription: cols.Length + " cols x " + list.Count + " rows", group: MessageRelayGroup);
            }
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns parsed instances of <c>T</c>. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="command">An open DB command.</param>
        /// <param name="readerParser">A parser that reads directly from an open data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <returns>Parsed instances of <c>T</c>.</returns>
        public static async Task<IList<T>> ParseRowsAsync<T>(DbCommand command, Func<IDataReader, T> readerParser, DbCapture dbCapture = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<T>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                var cols = reader.GetDataColumns();
                var outParams = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();

                SystemMessageRelay.RelayMessage("out params: " + (outParams.Length != 0 ? string.Join(", " + outParams.Select(p => p.ParameterName + " = " + Sqlize(p.Value))) : "none"), group: MessageRelayGroup);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = cols;
                    dbCapture.OutputParameters = outParams;
                }
                while (await reader.ReadAsync())
                {
                    list.Add(readerParser.Invoke(reader));
                }

                SystemMessageRelay.RelayMethodReturn(returnDescription: cols.Length + " cols x " + list.Count + " rows", group: MessageRelayGroup);
            }

            return list;
        }

        private static Regex SimpleColumnNamePattern { get; } = new Regex("^[A-Z0-9_]+$", RegexOptions.IgnoreCase);

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
            if (SimpleColumnNamePattern.IsMatch(columnName))
                return columnName;
            switch (provider)
            {
                case DbProvider.SqlServer:
                    return "[" + columnName + "]";
                case DbProvider.Oracle:
                    return "\"" + columnName + "\"";
                default:
                    return columnName;
            }
        }

        /// <summary>
        /// Prepare a value for insertion into a SQL statement.
        /// </summary>
        /// <param name="obj">A value.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A value as it might appear in a SQL expression</returns>
        public static string Sqlize(object obj, DbProvider provider = default)
        {
            if (obj == null || obj is DBNull) return "NULL";
            if (obj is bool boolValue) obj = boolValue ? 1 : 0;
            if (obj is byte || obj is short || obj is int || obj is long || obj is float || obj is double || obj is decimal) return obj.ToString();
            if (obj is DateTime dateTimeValue)
            {
                string dateTimeFormat;
                switch (provider)
                {
                    case DbProvider.SqlServer:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";  // format inspired by Microsoft SQL Server Managament Studio (SSMS)
                        break;
                    case DbProvider.Oracle:
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
                return sqlLiteral.Render();
            }
            var returnValue = "'" + obj.ToString().Replace("'", "''") + "'";
            if (provider == DbProvider.SqlServer && !TextUtil.IsAsciiPrintable(returnValue))
            {
                return "N" + returnValue;
            }
            return returnValue;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *           MISCELLANEOUS             *
         * * * * * * * * * * * * * * * * * * * */
        internal static object WrangleParameterValue(object value)
        {
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
                    _width = ValueUtil.Display(row[i]).Length;
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
                    sb.Append(ValueUtil.Display(row[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds a provider-specific statement to delete one or more rows from a table or drop (truncate) the table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="drop">If <c>true</c>, returns a statement to delete the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, returns a statement to delete the table database object (rather than just delete rows) and release the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL "DELETE" statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildDeleteStatement(string tableName, IFilter where, bool drop = false, bool purge = false, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                new Dictionary<string, object>
                {
                    { nameof(tableName), tableName },
                    { nameof(where), where },
                    { nameof(drop), drop },
                    { nameof(purge), purge },
                    { nameof(provider), provider }
                },
                group: MessageRelayGroup
            );

            string statement;

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
                    var ex = new DbException("This method is currently compatible only with certain providers: SqlServer, Oracle");
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                    throw ex;
            }

            SystemMessageRelay.RelayMethodReturnValue(statement, group: MessageRelayGroup);

            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to select one or more rows from a table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNames">The column name(s).</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="groupBy">The column name(s) to group by.</param>
        /// <param name="orderBy">The column name(s) to order by.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL "SELECT" statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildSelectStatement(string tableName, StringValues columnNames = default, IFilter where = null, StringValues groupBy = default, StringValues orderBy = default, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                new Dictionary<string, object>
                {
                    { nameof(tableName), tableName },
                    { nameof(columnNames), columnNames },
                    { nameof(where), where },
                    { nameof(groupBy), groupBy },
                    { nameof(orderBy), orderBy },
                    { nameof(provider), provider }
                },
                group: MessageRelayGroup
            );

            var strb = new StringBuilder("SELECT ")
                .AppendIf
                (
                    columnNames.Any(),
                    string.Join(", ", columnNames.Select(cn => RenderColumnName(cn, provider: provider))),
                    "*"
                )
                .Append(" FROM " + tableName)
                .AppendIf(where != null, () => " WHERE " + where.Render())
                .AppendIf(groupBy.Any(), () => " GROUP BY " + groupBy)
                .AppendIf(orderBy.Any(), () => " ORDER BY " + orderBy);

            SystemMessageRelay.RelayMethodReturnValue(() => strb.ToString(), group: MessageRelayGroup);
            return strb.ToString();
        }

        /// <summary>
        /// Builds a provider-specific statement to insert a row into a table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNamesAndValues">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildInsertStatement(string tableName, IEnumerable<DbParameter> columnNamesAndValues, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>(nameof(tableName), tableName)
                    .AppendRTL(columnNamesAndValues?.Select(c => new KeyValuePair<string, object>("[col]" + c.ParameterName, c.Value)))
                    .AppendRTL(nameof(provider), provider),
                group: MessageRelayGroup
            );

            string statement;

            switch (provider)
            {
                case DbProvider.SqlServer:
                    statement = "INSERT INTO " + tableName + " (" + string.Join(", ", columnNamesAndValues.Select(c => RenderColumnName(c, provider: DbProvider.SqlServer))) + ")";
                    statement += " VALUES (" + string.Join(", ", columnNamesAndValues.Select(c => Sqlize(c.Value, provider: DbProvider.SqlServer))) + ")";
                    break;
                case DbProvider.Oracle:
                    statement = "INSERT INTO " + tableName + " (" + string.Join(", ", columnNamesAndValues.Select(c => RenderColumnName(c, provider: DbProvider.Oracle))) + ")";
                    statement += " VALUES (" + string.Join(", ", columnNamesAndValues.Select(c => Sqlize(c.Value, provider: DbProvider.Oracle))) + ");";
                    break;
                default:
                    throw new DbException("This method is currently compatible only with certain providers: SqlServer, Oracle");
            }

            SystemMessageRelay.RelayMethodReturnValue(statement, group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_INSERT_STATEMENT);
            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to insert a row into a table and get the resulting identity.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="altGetIdentitySql">An optional alternate SELECT statement for retrieving the identity of the inserted row.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildInsertAndGetIdentityStatements(string tableName, IEnumerable<DbParameter> columns, string altGetIdentitySql = null, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>(nameof(tableName), tableName)
                    .AppendRTL(columns?.Select(c => new KeyValuePair<string, object>("[col]" + c.ParameterName, c.Value)))
                    .AppendRTL(nameof(altGetIdentitySql), altGetIdentitySql)
                    .AppendRTL(nameof(provider), provider),
                group: MessageRelayGroup
            );

            string statement = BuildInsertStatement(tableName, columns, provider: provider);
            string getIdentityStatement;

            if (altGetIdentitySql != null)
            {
                getIdentityStatement = altGetIdentitySql;
            }
            else
            {
                switch (provider)
                {
                    case DbProvider.SqlServer:
                        getIdentityStatement = "SELECT " + SqlLiteral.Identity(DbProvider.SqlServer);
                        break;
                    case DbProvider.Oracle:
                        getIdentityStatement = "SELECT " + SqlLiteral.Identity(DbProvider.Oracle) + ";";
                        break;
                    default:
                        throw new DbException("This method is currently compatible only with certain providers: SqlServer, Oracle");
                }
            }

            SystemMessageRelay.RelayMethodReturnValue(getIdentityStatement, group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_INSERT_GET_IDENTIY_STATEMENT);
            return statement + " " + getIdentityStatement;
        }

        /// <summary>
        /// Builds a provider-specific statement to update one or more rows in a table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildUpdateStatement(string tableName, IEnumerable<DbParameter> columns, IFilter where, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>(nameof(tableName), tableName)
                    .AppendRTL(columns?.Select(c => new KeyValuePair<string, object>("[col]" + c.ParameterName, c.Value)))
                    .AppendRTL(nameof(where), where)
                    .AppendRTL(nameof(provider), provider),
                group: MessageRelayGroup
            );

            string statement;

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
                    throw new DbException("This method requires a non-neutral DB provider.");
            }

            SystemMessageRelay.RelayMethodReturnValue(statement, group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_UPDATE_STATEMENT);
            return statement;
        }

        /// <summary>
        /// Builds a provider-specific statement to call a function.
        /// </summary>
        /// <param name="functionName">A function name.</param>
        /// <param name="parameters">Optional parameter(s) to pass into the function.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="DbException"></exception>
        public static string BuildFunctionStatement(string functionName, IEnumerable<DbParameter> parameters = null, DbProvider provider = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>(nameof(functionName), functionName)
                    .AppendRTL(parameters?.Select(p => new KeyValuePair<string, object>("[param]" + p.ParameterName, p.Value)))
                    .AppendRTL(nameof(provider), provider),
                group: MessageRelayGroup
            );

            string statement; 

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
                    throw new DbException("This method is currently compatible only with certain providers: SqlServer, Oracle");
            }

            SystemMessageRelay.RelayMethodReturnValue(statement, group: MessageRelayGroup, id: DbConstants.MessageRelay.GENERATED_FUNCTION_STATEMENT);
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
        /// Applies <c>AutoTruncate</c> on scalar query results e.g. replacing <c>DBNull</c> with <c>null</c>.
        /// </summary>
        /// <param name="result">A scalar query result</param>
        /// <param name="autoTrunc">Instructs certain processes how to handle <c>string</c> data</param>
        /// <returns></returns>
        public static object ProcessScalarResult(object result, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>(nameof(result), result)
                    .AppendRTL(nameof(autoTrunc), autoTrunc),
                group: MessageRelayGroup
            );

            if (ObjectUtil.IsNull(result))
            {
                SystemMessageRelay.RelayMethodReturnValue(null, group: MessageRelayGroup);
                return null;
            }
            if (result is string stringValue)
            {
                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                {
                    if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            if (stringValue.Length == 0)
                            {
                                SystemMessageRelay.RelayMethodReturnValue(null, group: MessageRelayGroup);
                                return null;
                            }
                        }
                        else
                            stringValue = stringValue.Trim();
                    }
                    else
                    {
                        stringValue = stringValue.Trim();
                        if (stringValue.Length == 0)
                        {
                            SystemMessageRelay.RelayMethodReturnValue(null, group: MessageRelayGroup);
                            return null;
                        }
                    }
                }
                else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    stringValue = stringValue.Trim();

                SystemMessageRelay.RelayMethodReturnValue(stringValue, group: MessageRelayGroup);
                return stringValue;
            }
            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }
    }
}
