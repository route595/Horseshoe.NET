using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.SqlDb
{
    public static class SqlDbUtil
    {
        public static string BuildConnectionString(string dataSource, string initialCatalog, bool hasCredentials, IDictionary<string,string> additionalConnectionAttributes)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("Data Source=" + dataSource.Replace(":", ","));  // DBSVR02:9999 -> Data Source=DBSVR02,9999;...

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

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(SqlDbSettings.DefaultDataSource, SqlDbSettings.DefaultInitialCatalog, SqlDbSettings.DefaultCredentials.HasValue, SqlDbSettings.DefaultAdditionalConnectionAttributes);
        }

        public static SqlConnection LaunchConnection
        (
            SqlDbConnectionInfo connectionInfo = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig());
            resultantConnectionInfo?.Invoke(connectionInfo);

            var conn = connectionInfo.SqlCredentials != null
                ? new SqlConnection(connectionInfo.ConnectionString, connectionInfo.SqlCredentials)
                : new SqlConnection(connectionInfo.ConnectionString);
            conn.Open();
            return conn;
        }

        internal static SqlCommand BuildCommand
        (
            SqlConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null
        )
        {
            var cmd = new SqlCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
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
            if ((timeout ?? SqlDbSettings.DefaultTimeout).TryHasValue(out int value))
            {
                cmd.CommandTimeout = value;
            }
            modifyCommand?.Invoke(cmd);
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
