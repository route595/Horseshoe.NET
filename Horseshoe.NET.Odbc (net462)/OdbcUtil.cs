using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Odbc
{
    public static class OdbcUtil
    {
        public static string BuildConnectionString(string dataSource, Credential? credentials = null, IDictionary<string, string> additionalConnectionAttributes = null, int? connectionTimeout = null)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("DSN=" + dataSource);

            // credentials
            if (credentials.HasValue)
            {
                sb.Append(";UID=" + credentials.Value.UserName);
                sb.Append(";PWD=" + credentials.Value.Password.ToUnsecurePassword());
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
            // ref https://stackoverflow.com/questions/20142746/what-is-connect-timeout-in-sql-server-connection-string
            if (ValueUtil.TryHasValue(connectionTimeout, out int value))
            {
                sb.Append(";Connection Timeout=" + value);
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(OdbcSettings.DefaultDataSource, credentials: OdbcSettings.DefaultCredentials, additionalConnectionAttributes: OdbcSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: OdbcSettings.DefaultConnectionTimeout);
        }

        /// <summary>
        /// Creates an open DB connection ready to send queries, updates, etc.
        /// </summary>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="cryptoOptions">Optional options for the crypto engine to decrypt the connection string password. Source should be DB settings.</param>
        /// <returns>A new DB connection</returns>
        public static OdbcConnection LaunchConnection
        (
            OdbcConnectionInfo connectionInfo = null,
            Action<OdbcConnection> peekConnection = null,
            CryptoOptions cryptoOptions = null
        )
        {
            connectionInfo = DbUtil.LoadFinalConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), cryptoOptions: cryptoOptions);
            var conn = new OdbcConnection(connectionInfo.ConnectionString);
            conn.Open();
            peekConnection?.Invoke(conn);
            return conn;
        }

        internal static OdbcCommand BuildTextCommand
        (
            OdbcConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OdbcTransaction transaction,
            int? commandTimeout,
            Action<OdbcCommand> peekCommand
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

        internal static OdbcCommand BuildProcedureCommand
        (
            OdbcConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            OdbcTransaction transaction,
            int? commandTimeout,
            Action<OdbcCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                commandText,
                CommandType.StoredProcedure,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        internal static OdbcCommand BuildCommand
        (
            OdbcConnection conn,
            string commandText,
            CommandType commandType,
            IEnumerable<DbParameter> parameters,
            OdbcTransaction transaction,
            int? commandTimeout,
            Action<OdbcCommand> peekCommand
        )
        {
            var cmd = new OdbcCommand
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
                    if (param is OdbcParameter)
                    {
                        cmd.Parameters.Add(param);
                    }
                    else
                    {
                        var odbcParam = new OdbcParameter(param.ParameterName, param.Value)
                        {
                            Direction = param.Direction,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion,
                            DbType = param.DbType,
                            IsNullable = param.IsNullable
                        };
                        if (param is Parameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                odbcParam.DbType = uparam.DbType;
                            }
                        }
                        cmd.Parameters.Add(odbcParam);
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
    }
}
