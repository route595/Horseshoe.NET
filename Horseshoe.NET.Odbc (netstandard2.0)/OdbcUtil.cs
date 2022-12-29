using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

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
            if (connectionTimeout.HasValue)
            {
                sb.Append(";Connection Timeout=" + connectionTimeout);
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
            CryptoOptions cryptoOptions = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal: journal);
            var conn = new OdbcConnection
            (
                connectionInfo.IsEncryptedPassword
                  ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: cryptoOptions)
                  : connectionInfo.ConnectionString
            );
            conn.Open();
            return conn;
        }

        internal static OdbcCommand BuildTextCommand
        (
            OdbcConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OdbcCommand> alterCommand
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

        internal static OdbcCommand BuildProcedureCommand
        (
            OdbcConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OdbcCommand> alterCommand
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

        internal static OdbcCommand BuildCommand
        (
            OdbcConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OdbcCommand> alterCommand
        )
        {
            var cmd = new OdbcCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
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
            alterCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
