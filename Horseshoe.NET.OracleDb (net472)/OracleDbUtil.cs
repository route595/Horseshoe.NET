using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.OracleDb.Meta;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Horseshoe.NET.OracleDb
{
    public static class OracleDbUtil
    {
        private static string MessageRelayGroup => OracleDbConstants.MessageRelayGroup;

        //private static Regex dbServerPattern = new Regex("^[a-z0-9-_]+(\\.[a-z0-9-_]+)*$", RegexOptions.IgnoreCase);
        //private static Regex dbServerPortPattern = new Regex("^[a-z0-9-_]+(\\.[a-z0-9-_]+)*[:][0-9]+$", RegexOptions.IgnoreCase);

        public static string BuildConnectionString(OraServer dataSource = null, bool hasCredentials = false, IDictionary<string, string> additionalConnectionAttributes = null, int? connectionTimeout = null)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("Data Source=");  // e.g. Server.DataSource (includes port)
            if (dataSource.ServiceName == null && dataSource.InstanceName == null)
            {
                // basic style                                    // e.g. Data Source=MY_ORA_SERVER[:1234]
                sb.Append(dataSource.DataSource);                 //      Data Source=MY_TNS_ALIAS
            }
            else 
            {
                // data source continued - EZ connect style       // e.g. Data Source=//MY_ORA_SERVER[:1234]//MY_INSTANCE
                sb.Append("//").Append(dataSource.DataSource);    //      Data Source=//MY_ORA_SERVER[:1234]/MY_SERVICE/MY_INSTANCE

                // service name
                sb.Append('/');
                sb.AppendIf(dataSource.ServiceName != null, dataSource.ServiceName);

                // instance name
                if (dataSource.InstanceName != null)
                {
                    sb.Append('/').Append(dataSource.InstanceName);
                }
            }

            // integrated security
            if (!hasCredentials)
            {
                sb.Append(";Integrated Security=SSPI");
            }

            // additional attributes
            if (additionalConnectionAttributes != null)
            {
                foreach (var kvp in additionalConnectionAttributes)
                {
                    sb.Append(";" + kvp.Key + "=" + kvp.Value);
                }
            }

            // additional attributes
            // ref https://learn.microsoft.com/en-us/dotnet/api/system.data.oracleclient.oracleconnection.connectiontimeout?view=netframework-4.8
            if (connectionTimeout.HasValue)
            {
                sb.Append(";Connection Timeout=" + connectionTimeout);
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(OracleDbSettings.DefaultDataSource, OracleDbSettings.DefaultCredentials.HasValue, additionalConnectionAttributes: OracleDbSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: OracleDbSettings.DefaultConnectionTimeout);
        }

        /// <summary>
        /// Creates an open DB connection ready to send queries, updates, etc.
        /// </summary>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="journal"></param>
        /// <returns>A new DB connection</returns>
        public static OracleConnection LaunchConnection
        (
            OracleDbConnectionInfo connectionInfo = null,
            Action<OracleConnection> peekConnection = null,
            CryptoOptions cryptoOptions = null
        )
        {
            connectionInfo = DbUtil.LoadFinalConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), cryptoOptions: cryptoOptions);
            var conn = connectionInfo.OracleCredentials != null
                ? new OracleConnection(connectionInfo.ConnectionString, connectionInfo.OracleCredentials)
                : new OracleConnection
                  (
                      connectionInfo.IsEncryptedPassword
                        ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: OracleDbSettings.CryptoOptions)
                        : connectionInfo.ConnectionString
                  );
            conn.Open();
            if (connectionInfo.AutoClearConnectionPool || OracleDbSettings.AutoClearConnectionPool)
            {
                // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
                OracleConnection.ClearPool(conn);   // will clear on close
            }
            peekConnection?.Invoke(conn);
            return conn;
        }

        //public static OracleConnection LaunchConnection
        //(
        //    OracleConnection conn,
        //    bool autoClearConnectionPool = false
        //)
        //{
        //    conn = conn.Credential != null
        //        ? new OracleConnection(conn.ConnectionString, conn.Credential)
        //        : new OracleConnection(conn.ConnectionString);
        //    conn.Open();
        //    if (autoClearConnectionPool || OracleDbSettings.AutoClearConnectionPool)
        //    {
        //        // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
        //        OracleConnection.ClearPool(conn);   // will clear on close
        //    }
        //    return conn;
        //}

        internal static OracleCommand BuildTextCommand
        (
            OracleConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OracleTransaction transaction,
            int? commandTimeout,
            Action<OracleCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                commandText,
                CommandType.Text,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        internal static OracleCommand BuildProcedureCommand
        (
            OracleConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OracleTransaction transaction,
            int? commandTimeout,
            Action<OracleCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                commandText,
                CommandType.StoredProcedure,
                parameters: parameters,
                transaction : transaction,
                commandTimeout : commandTimeout,
                peekCommand: peekCommand
            );
        }

        public static OracleCommand BuildCommand
        (
            OracleConnection conn,
            string commandText,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            OracleTransaction transaction = null,
            int? commandTimeout = null,
            Action<OracleCommand> peekCommand = null
        )
        {
            var cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = commandText,
                CommandType = commandType,
                Transaction = transaction
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OracleParameter oraParam)
                    {
                        cmd.Parameters.Add(oraParam);
                    }
                    else
                    {
                        cmd.Parameters.Add(param.ToOracleParameter());
                    }
                }
            }
            if (ValueUtil.TryHasValue(commandTimeout, out int value))
            {
                cmd.CommandTimeout = value;
            }
            peekCommand?.Invoke(cmd);
            return cmd;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *  ORACLE SPECIFIC METHODS / OBJECTS  *
         * * * * * * * * * * * * * * * * * * * */

        public static OracleParameter BuildInParam(string name = null, object value = null)
        {
            return new OracleParameter(name, value ?? DBNull.Value) { Direction = ParameterDirection.Input };
        }

        public static OracleParameter BuildVarchar2InParam(string name = null, string value = null)
        {
            return new OracleParameter(name, OracleDbType.Varchar2, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildVarchar2OutParam(string name = null, int length = 100)
        {
            return new OracleParameter(name, OracleDbType.Varchar2, length) { Direction = ParameterDirection.Output };
        }

        public static OracleParameter BuildIntInParam(string name = null, int? value = null)
        {
            return new OracleParameter(name, OracleDbType.Int32, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildIntOutParam(string name = null)
        {
            return new OracleParameter(name, OracleDbType.Int32, ParameterDirection.Output);
        }

        public static OracleParameter BuildDateInParam(string name = null, DateTime? value = null)
        {
            return new OracleParameter(name, OracleDbType.Date, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildDateOutParam(string name = null)
        {
            return new OracleParameter(name, OracleDbType.Date, ParameterDirection.Output);
        }

        public static OracleParameter BuildOutRefCursor(string name = null)
        {
            return new OracleParameter(name, OracleDbType.RefCursor, ParameterDirection.Output);
        }

        public static string ZapString(object obj)
        {
            if (obj is OracleString oracleString)
                return Zap.String(oracleString.Value);
            return Zap.String(obj);
        }

        public static object NormalizeDbValue(object obj)
        {
            if (obj == null || obj is DBNull)
                return null;
            // string
            if (obj is OracleString oracleString)
                return oracleString.Value;
            if (obj is OracleXmlType oracleXml)
                return oracleXml.Value;
            if (obj is OracleXmlStream oracleXmlStream)
                return oracleXmlStream.Value;
            if (obj is OracleClob oracleClob)
                return oracleClob.Value;
            // date/time/timespan
            if (obj is OracleDate oracleDate)
                return oracleDate.Value;
            if (obj is OracleTimeStamp oracleTimeStamp)
                return oracleTimeStamp.Value;
            if (obj is OracleTimeStampLTZ oracleTimeStampLTZ)
                return oracleTimeStampLTZ.Value;
            if (obj is OracleTimeStampTZ oracleTimeStampTZ)
                return oracleTimeStampTZ.Value;
            if (obj is OracleIntervalDS oracleIntervalDS)
                return oracleIntervalDS.Value;
            // bool
            if (obj is OracleBoolean oracleBool)
                return oracleBool.Value;
            // numeric
            if (obj is OracleDecimal oracleDecimal)
                return oracleDecimal.Value;
            if (obj is OracleIntervalYM oracleIntervalYM)
                return oracleIntervalYM.Value;
            // bytes
            if (obj is OracleBinary oracleBinary)
                return oracleBinary.Value;
            if (obj is OracleBlob oracleBlob)
                return oracleBlob.Value;
            if (obj is OracleBFile oracleBFile)
                return oracleBFile.Value;
            return obj;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Autoconverts <c>DBNull</c> to <c>null</c> and Oracle types
        /// to their common equivalents, e.g. <c>OracleString</c> to <c>string</c>, etc.
        /// </summary>
        /// <param name="reader">An oben data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IEnumerable<object[]> ReadAsObjects(IDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<object[]>();
            var cols = reader.GetDataColumns();
            if (dbCapture != null)
                dbCapture.DataColumns = cols;
            while (reader.Read())
            {
                var objects = GetAllRowValues(reader, cols.Length, autoTrunc: autoTrunc);
                list.Add(objects);
            }

            SystemMessageRelay.RelayMethodReturn(returnDescription: "list count: " + list.Count, group: MessageRelayGroup);
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Autoconverts <c>DBNull</c> to <c>null</c> and Oracle types
        /// to their common equivalents, e.g. <c>OracleString</c> to <c>string</c>, etc.
        /// </summary>
        /// <param name="reader">An oben data reader.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IEnumerable<object[]>> ReadAsObjectsAsync(IDataReader reader, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var list = new List<object[]>();
            var cols = reader.GetDataColumns();
            if (dbCapture != null)
                dbCapture.DataColumns = cols;
            while (await ((DbDataReader)reader).ReadAsync())
            {
                var objects = await GetAllRowValuesAsync(reader, cols.Length, autoTrunc: autoTrunc);
                list.Add(objects);
            }

            SystemMessageRelay.RelayMethodReturn(returnDescription: "list count: " + list.Count, group: MessageRelayGroup);
            return list;
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static IEnumerable<object[]> ReadAsObjects(IDbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var reader = command.ExecuteReader())
            {
                var list = ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);
                if (dbCapture != null)
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Reads data from a reader and returns the raw objects. Instances of <c>DBNull</c> are returned as <c>null</c>.
        /// </summary>
        /// <param name="command">An open DB command.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution</param>
        /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>Rows of data as <c>object[]</c>s.</returns>
        public static async Task<IEnumerable<object[]>> ReadAsObjectsAsync(IDbCommand command, DbCapture dbCapture = null, AutoTruncate autoTrunc = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var reader = await ((DbCommand)command).ExecuteReaderAsync())
            {
                var list = await ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);
                if (dbCapture != null)
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<DbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Gets all field values from the current row of an open <c>IDataReader</c> as an <c>object[]</c>.
        /// </summary>
        /// <param name="reader">a data reader</param>
        /// <param name="columnCount">the number of columns</param>
        /// <param name="autoTrunc">how to handle strings</param>
        /// <returns></returns>
        public static object[] GetAllRowValues(IDataReader reader, int columnCount, AutoTruncate autoTrunc = default)
        {
            var items = new object[columnCount];

            for (int i = 0; i < items.Length; i++)
            {
                if (reader.IsDBNull(i))
                    continue;
                items[i] = NormalizeDbValue(reader[i]);
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
        /// <param name="reader">a data reader</param>
        /// <param name="columnCount">the number of columns</param>
        /// <param name="autoTrunc">how to handle strings</param>
        /// <returns></returns>
        public static async Task<object[]> GetAllRowValuesAsync(IDataReader reader, int columnCount, AutoTruncate autoTrunc = default)
        {
            var items = new object[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                if (reader is DbDataReader dbDataReader && await dbDataReader.IsDBNullAsync(i))
                    continue;
                items[i] = NormalizeDbValue(reader[i]);
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
    }
}
