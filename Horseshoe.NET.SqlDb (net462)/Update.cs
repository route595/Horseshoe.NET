using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<DbParameter> columns, 
            SqlDbConnectionInfo connectionInfo = null, 
            Filter where = null, 
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            SqlConnection conn, 
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where = null, 
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString(platform: DbPlatform.SqlServer)));
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
