using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

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
            if (connectionTimeout.HasValue)
            {
                sb.Append(";Connect Timeout=" + connectionTimeout);
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
        /// Creates and opens a DB connection from the supplied connection information
        /// </summary>
        /// <param name="connectionInfo">Connection information.</param>
        /// <param name="cryptoOptions">Options for decrypting DB passwords.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns></returns>
        public static OleDbConnection LaunchConnection
        (
            OleDbConnectionInfo connectionInfo = null, 
            CryptoOptions cryptoOptions = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal: journal);
            var conn = new OleDbConnection
            (
                connectionInfo.IsEncryptedPassword
                  ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: cryptoOptions)
                  : connectionInfo.ConnectionString
            );
            conn.Open();
            return conn;
        }

        internal static OleDbCommand BuildTextCommand
        (
            OleDbConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OleDbCommand> alterCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.Text,
                commandText,
                parameters,
                commandTimeout,
                alterCommand
            );
        }

        internal static OleDbCommand BuildProcedureCommand
        (
            OleDbConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OleDbCommand> alterCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.StoredProcedure,
                commandText,
                parameters,
                commandTimeout,
                alterCommand
            );
        }

        private static OleDbCommand BuildCommand
        (
            OleDbConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OleDbCommand> alterCommand
        )
        {
            var cmd = new OleDbCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
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
            alterCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
