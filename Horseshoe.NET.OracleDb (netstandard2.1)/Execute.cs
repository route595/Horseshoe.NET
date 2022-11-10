using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using Horseshoe.NET.Db;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter>? parameters = null, 
            OracleDbConnectionInfo? connectionInfo = null, 
            DbCapture? dbCapture = null,
            int? timeout = null,
            Action<OracleCommand>? modifyCommand = null,
            Action<OracleDbConnectionInfo>? resultantConnectionInfo = null
        )
        {
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, timeout: timeout, modifyCommand: modifyCommand);
            }
        }

        public static int StoredProcedure
        (
            OracleConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter>? parameters = null,
            DbCapture? dbCapture = null,
            int? timeout = null,
            Action<OracleCommand>? modifyCommand = null
        )
        {
            using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                modifyCommand?.Invoke(cmd);
                var returnValue = cmd.ExecuteNonQuery();
                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<OracleParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                        .Select(p => new Parameter(p.ParameterName, p.Value))
                        .ToArray();
                }
                return returnValue;
            }
        }

        public static int SQL
        (
            string statement, 
            IEnumerable<DbParameter>? parameters = null, 
            OracleDbConnectionInfo? connectionInfo = null,
            int? timeout = null,
            Action<OracleCommand>? modifyCommand = null,
            Action<OracleDbConnectionInfo>? resultantConnectionInfo = null
        )
        {
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);   // parameters optional here, e.g. may already be included in the SQL statement
            }
        }

        public static int SQL
        (
            OracleConnection conn, 
            string statement, 
            IEnumerable<DbParameter>? parameters = null,
            int? timeout = null,
            Action<OracleCommand>? modifyCommand = null
        )
        {
            using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                modifyCommand?.Invoke(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
