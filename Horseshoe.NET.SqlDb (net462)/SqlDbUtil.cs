﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

using Horseshoe.NET.Db;
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
            if (connectionTimeout.HasValue)
            {
                sb.Append(";Connect Timeout = " + connectionTimeout);
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(SqlDbSettings.DefaultDataSource, initialCatalog: SqlDbSettings.DefaultInitialCatalog, hasCredentials: SqlDbSettings.DefaultCredentials.HasValue, additionalConnectionAttributes: SqlDbSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: SqlDbSettings.DefaultConnectionTimeout);
        }

        public static SqlConnection LaunchConnection
        (
            SqlDbConnectionInfo connectionInfo = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal: journal);
            var conn = connectionInfo.SqlCredentials != null
                ? new SqlConnection(connectionInfo.ConnectionString, connectionInfo.SqlCredentials)
                : new SqlConnection(connectionInfo.ConnectionString);
            conn.Open();
            return conn;
        }

        internal static SqlCommand BuildTextCommand
        (
            SqlConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<SqlCommand> alterCommand
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

        internal static SqlCommand BuildProcedureCommand
        (
            SqlConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<SqlCommand> alterCommand
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

        internal static SqlCommand BuildCommand
        (
            SqlConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
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
            if (commandTimeout.TryHasValue(out int value))
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
