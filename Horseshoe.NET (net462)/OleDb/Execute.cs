using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

namespace Horseshoe.NET.OleDb
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            OleDbConnectionInfo connectionInfo = null, 
            DbCapture dbCapture = null,
            int? timeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> modifyCommand = null,
            Action<OleDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, timeout: timeout, modifyCommand: modifyCommand);
            }
        }

        public static int StoredProcedure
        (
            OleDbConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null,
            DbCapture dbCapture = null,
            int? timeout = null,
            Action<OleDbCommand> modifyCommand = null
        )
        {
            using (var cmd = OleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                modifyCommand?.Invoke(cmd);
                var returnValue = cmd.ExecuteNonQuery();
                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<OleDbParameter>()
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
            OleDbConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> modifyCommand = null,
            Action<OleDbConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);   // parameters optional here, e.g. may already be included in the SQL statement
            }
        }

        public static int SQL
        (
            OleDbConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<OleDbCommand> modifyCommand = null
        )
        {
            using (var cmd = OleDbUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                modifyCommand?.Invoke(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
