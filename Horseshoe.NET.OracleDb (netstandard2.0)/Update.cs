using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<DbParameter> columns, 
            OracleDbConnectionInfo connectionInfo = null, 
            Filter where = null, 
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OracleConnection conn, 
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where = null, 
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString(platform: DbPlatform.Oracle)));
            if (where != null)
            {
                statement += @"
                WHERE " + where;
            }

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            return Execute.SQL(conn, statement, timeout: timeout, modifyCommand: modifyCommand);
        }
    }
}
