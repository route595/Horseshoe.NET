using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// A suite of factory and utility methods for <c>Horseshoe.NET.OleDb</c>
    /// </summary>
    public static class OleDbUtil
    {
        /// <summary>
        /// Builds a connection string for OLE DB from the supplied data source and other parts.
        /// </summary>
        /// <param name="dataSource">An OLE DB data source name or DNS alias</param>
        /// <param name="credentials">User name and password.</param>
        /// <param name="additionalConnectionAttributes">Additional connection attributes.</param>
        /// <param name="connectionTimeout">The time to wait while trying to establish a connection before terminating the attempt and generating an error.</param>
        /// <returns></returns>
        public static string BuildConnectionString(string dataSource, Credential? credentials = null, IDictionary<string, string> additionalConnectionAttributes = null, int? connectionTimeout = null)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("Data Source=" + dataSource);

            // credentials
            if (credentials.HasValue)
            {
                sb.Append(";User ID=" + credentials.Value.UserName);
                sb.Append(";Password=" + credentials.Value.Password.ToUnsecurePassword());
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
            // ref https://learn.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbconnection.connectiontimeout?view=dotnet-plat-ext-6.0
            if (ValueUtil.TryHasValue(connectionTimeout, out int value))
            {
                sb.Append(";" + (OleDbSettings.PreferConnectTimeoutInGeneratedConnectionString ? "Connect" : "Connection") + " Timeout=" + value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Builds a connection string piecemeal from configuration
        /// </summary>
        /// <returns></returns>
        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(OleDbSettings.DefaultDataSource, credentials: OleDbSettings.DefaultCredentials, additionalConnectionAttributes: OleDbSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: OleDbSettings.DefaultConnectionTimeout);
        }

        /// <summary>
        /// Creates an open DB connection ready to send queries, updates, etc.
        /// </summary>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="journal"></param>
        /// <returns>A new DB connection</returns>
        public static OleDbConnection LaunchConnection
        (
            OleDbConnectionInfo connectionInfo = null,
            Action<OleDbConnection> peekConnection = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadFinalConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal: journal);
            var conn = new OleDbConnection
            (
                connectionInfo.IsEncryptedPassword
                  ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: OleDbSettings.CryptoOptions)
                  : connectionInfo.ConnectionString
            );
            conn.Open();
            peekConnection?.Invoke(conn);
            return conn;
        }

        internal static OleDbCommand BuildTextCommand
        (
            OleDbConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OleDbTransaction transaction,
            int? commandTimeout,
            Action<OleDbCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.Text,
                commandText,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        internal static OleDbCommand BuildProcedureCommand
        (
            OleDbConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OleDbTransaction transaction,
            int? commandTimeout,
            Action<OleDbCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.StoredProcedure,
                commandText,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        private static OleDbCommand BuildCommand
        (
            OleDbConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OleDbTransaction transaction,
            int? commandTimeout,
            Action<OleDbCommand> peekCommand
        )
        {
            var cmd = new OleDbCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText,
                Transaction = transaction
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OleDbParameter oleParam)
                    {
                        cmd.Parameters.Add(oleParam);
                    }
                    else
                    {
                        var oledbParam = new OleDbParameter(param.ParameterName, param.Value)
                        {
                            Direction = param.Direction,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion,
                            IsNullable = param.IsNullable
                        };
                        if (param is Parameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                oledbParam.DbType = uparam.DbType;
                            }
                        }
                        cmd.Parameters.Add(oledbParam);
                    }
                }
            }
            if (commandTimeout.TryHasValue(out int value))
            {
                cmd.CommandTimeout = value;
            }
            peekCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
