using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class OdbcUtil
    {
        public static string BuildConnectionString(string dataSource, Credential? credentials, IDictionary<string, string> additionalConnectionAttributes, CryptoOptions cryptoOptions)
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
                if (credentials.Value.HasSecurePassword)
                {
                    sb.Append(";PWD=" + credentials.Value.SecurePassword.ToUnsecureString());
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    sb.Append(";PWD=" + Decrypt.String(credentials.Value.Password, options: cryptoOptions));
                }
                else if (credentials.Value.Password != null)
                {
                    sb.Append(";PWD=" + credentials.Value.Password);
                }
            }

            // additional attributes
            if (additionalConnectionAttributes != null)
            {
                foreach (var kvp in additionalConnectionAttributes)
                {
                    sb.Append(";" + kvp.Key + "=" + kvp.Value);
                }
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig(CryptoOptions cryptoOptions)
        {
            return BuildConnectionString(OdbcSettings.DefaultDataSource, OdbcSettings.DefaultCredentials, OdbcSettings.DefaultAdditionalConnectionAttributes, cryptoOptions);
        }

        public static OdbcConnection LaunchConnection
        (
            OdbcConnectionInfo connectionInfo = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(cryptoOptions: cryptoOptions));
            resultantConnectionInfo?.Invoke(connectionInfo);

            var conn = new OdbcConnection(connectionInfo.ConnectionString);
            conn.Open();
            return conn;
        }

        internal static OdbcCommand BuildCommand
        (
            OdbcConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<OdbcCommand> modifyCommand = null
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
            if ((timeout ?? OdbcSettings.DefaultTimeout).TryHasValue(out int value))
            {
                cmd.CommandTimeout = value;
            }
            modifyCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
