using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;

namespace Horseshoe.NET.Odbc
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            OdbcConnectionInfo connectionInfo = null, 
            DbCapture dbCapture = null,
            int? timeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, timeout: timeout, modifyCommand: modifyCommand);
            }
        }

        public static int StoredProcedure
        (
            OdbcConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null,
            DbCapture dbCapture = null,
            int? timeout = null,
            Action<OdbcCommand> modifyCommand = null
        )
        {
            using (var cmd = OdbcUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                modifyCommand?.Invoke(cmd);
                var returnValue = cmd.ExecuteNonQuery();
                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<OdbcParameter>()
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
            OdbcConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);   // parameters optional here, e.g. may already be included in the SQL statement
            }
        }

        public static int SQL
        (
            OdbcConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null,
            Action<OdbcCommand> modifyCommand = null
        )
        {
            using (var cmd = OdbcUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                modifyCommand?.Invoke(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
