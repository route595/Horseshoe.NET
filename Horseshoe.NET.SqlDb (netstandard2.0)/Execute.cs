using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.SqlDb
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            SqlDbConnectionInfo connectionInfo = null, 
            DbCapture dbCapture = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, timeout: timeout, modifyCommand: modifyCommand);
            }
        }

        public static int StoredProcedure
        (
            SqlConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null,
            DbCapture dbCapture = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null
        )
        {
            using (var cmd = SqlDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                modifyCommand?.Invoke(cmd);
                var returnValue = cmd.ExecuteNonQuery();
                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<SqlParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }
                return returnValue;
            }
        }

        public static int SQL
        (
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            SqlDbConnectionInfo connectionInfo = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);   // parameters optional here, e.g. may already be included in the SQL statement
            }
        }

        public static int SQL
        (
            SqlConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null
        )
        {
            using (var cmd = SqlDbUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                modifyCommand?.Invoke(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
