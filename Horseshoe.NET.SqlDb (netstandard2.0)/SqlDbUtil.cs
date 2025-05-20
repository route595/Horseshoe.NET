using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    public static class SqlDbUtil
    {
        public static string BuildConnectionString(DbServer dataSource = null, string initialCatalog = null, bool hasCredentials = false, IDictionary<string,string> additionalConnectionAttributes = null, int? connectionTimeout = null)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("Data Source=" + dataSource.DataSource.Replace(":", ","));  // DBSVR02:9999 -> Data Source=DBSVR02,9999;

            // initial catalog
            if (initialCatalog != null)
            {
                sb.Append(";Initial Catalog=" + initialCatalog);
            }

            // integrated security
            if (!hasCredentials)
            {
                sb.Append(";Integrated Security=true");
            }

            // additional attributes
            if (additionalConnectionAttributes != null)
            {
                foreach (var kvp in additionalConnectionAttributes)
                {
                    sb.Append(";" + kvp.Key + "=" + kvp.Value);
                }
            }

            // timeout
            if (ValueUtil.TryHasValue(connectionTimeout, out int value))
            {
                sb.Append(";" + (SqlDbSettings.PreferConnectTimeoutInGeneratedConnectionString ? "Connect" : "Connection") + " Timeout = " + value);
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(SqlDbSettings.DefaultDataSource, initialCatalog: SqlDbSettings.DefaultInitialCatalog, hasCredentials: SqlDbSettings.DefaultCredentials.HasValue, additionalConnectionAttributes: SqlDbSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: SqlDbSettings.DefaultConnectionTimeout);
        }

        /// <summary>
        /// Creates an open DB connection ready to send queries, updates, etc.
        /// </summary>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="cryptoOptions"></param>
        /// <returns>A new DB connection</returns>
        public static SqlConnection LaunchConnection
        (
            SqlDbConnectionInfo connectionInfo = null,
            Action<SqlConnection> peekConnection = null,
            CryptoOptions cryptoOptions = null
        )
        {
            connectionInfo = DbUtil.LoadFinalConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), cryptoOptions: cryptoOptions);
            var conn = connectionInfo.SqlCredentials != null
                ? new SqlConnection(connectionInfo.ConnectionString, connectionInfo.SqlCredentials)
                : new SqlConnection
                  (
                      connectionInfo.IsEncryptedPassword 
                        ? DbUtil.DecryptInlinePassword(connectionInfo.ConnectionString, cryptoOptions: SqlDbSettings.CryptoOptions) 
                        : connectionInfo.ConnectionString
                  );
            conn.Open();
            peekConnection?.Invoke(conn);
            return conn;
        }

        internal static SqlCommand BuildTextCommand
        (
            SqlConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            SqlTransaction transaction,
            int? commandTimeout,
            Action<SqlCommand> peekCommand
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

        internal static SqlCommand BuildProcedureCommand
        (
            SqlConnection conn,
            string prodecureName,
            IEnumerable<DbParameter> parameters,
            SqlTransaction transaction,
            int? commandTimeout,
            Action<SqlCommand> peekCommand
        )
        {
            return BuildCommand
            (
                conn,
                prodecureName,
                CommandType.StoredProcedure,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: commandTimeout,
                peekCommand: peekCommand
            );
        }

        public static SqlCommand BuildCommand
        (
            SqlConnection conn,
            string commandText,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            SqlTransaction transaction = null,
            int? commandTimeout = null,
            Action<SqlCommand> peekCommand = null
        )
        {
            var cmd = new SqlCommand
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
                    if (param is SqlParameter sqlParam)
                    {
                        cmd.Parameters.Add(sqlParam);
                    }
                    else
                    {
                        cmd.Parameters.Add(param.ToSqlParameter());
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

        /* * * * * * * * * * * * * * * * * * * * * * 
         *  SQL SERVER SPECIFIC METHODS / OBJECTS  *
         * * * * * * * * * * * * * * * * * * * * * */

        public static SqlParameter BuildParam(string name = null, object value = null)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static SqlParameter BuildVarcharParam(string name = null, string value = null)
        {
            return new SqlParameter(name, value as object ?? DBNull.Value);
        }

        public static SqlParameter BuildIntParam(string name = null, int? value = null)
        {
            return new SqlParameter(name, value as object ?? DBNull.Value);
        }

        public static SqlParameter BuildDateParam(string name = null, DateTime? value = null)
        {
            return new SqlParameter(name, value as object ?? DBNull.Value);
        }
    }
}
