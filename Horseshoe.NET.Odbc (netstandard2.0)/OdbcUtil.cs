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

        public static OdbcConnection LaunchConnection
        (
            OdbcConnectionInfo connectionInfo = null,
            Action<OdbcConnection> peekConnection = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadFinalConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal: journal);
            var conn = new OdbcConnection
            (
                connectionInfo.IsEncryptedPassword
                  ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: OdbcSettings.CryptoOptions)
                  : connectionInfo.ConnectionString
            );
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
                CommandType.Text,
                commandText,
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
                CommandType.StoredProcedure,
                commandText,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        public static OdbcCommand BuildCommand
        (
            OdbcConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            OdbcTransaction transaction = null,
            int? commandTimeout = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            var cmd = new OdbcCommand
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
            if (commandTimeout.TryHasValue(out int value))
            {
                cmd.CommandTimeout = value;
            }
            peekCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
