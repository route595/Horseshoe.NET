using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OleDb
{
    public static class OleDbUtil
    {
        public static string BuildConnectionString(string dataSource, Credential? credentials, IDictionary<string, string> additionalConnectionAttributes, CryptoOptions cryptoOptions)
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
                if (credentials.Value.HasSecurePassword)
                {
                    sb.Append(";Password=" + credentials.Value.SecurePassword.ToUnsecureString());
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    sb.Append(";Password=" + Decrypt.String(credentials.Value.Password, options: cryptoOptions));
                }
                else if (credentials.Value.Password != null)
                {
                    sb.Append(";Password=" + credentials.Value.Password);
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
            return BuildConnectionString(OleDbSettings.DefaultDataSource, OleDbSettings.DefaultCredentials, OleDbSettings.DefaultAdditionalConnectionAttributes, cryptoOptions);
        }

        public static OleDbConnection LaunchConnection
        (
            OleDbConnectionInfo connectionInfo = null, 
            CryptoOptions cryptoOptions = null,
            Action<OleDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(cryptoOptions: cryptoOptions));
            resultantConnectionInfo?.Invoke(connectionInfo);

            var conn = new OleDbConnection(connectionInfo.ConnectionString);
            conn.Open();
            return conn;
        }

        internal static OleDbCommand BuildCommand
        (
            OleDbConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<OleDbCommand> modifyCommand = null
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
            if ((timeout ?? OleDbSettings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? OleDbSettings.DefaultTimeout).Value;
            }
            modifyCommand?.Invoke(cmd);
            return cmd;
        }
    }
}
