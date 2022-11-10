using System;
using System.Data.SqlClient;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Delete
    {
        public static int Table
        (
            string tableName, 
            SqlDbConnectionInfo connectionInfo = null, 
            Filter where = null, 
            bool truncate = false,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, where: where, truncate: truncate, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            SqlConnection conn, 
            string tableName, 
            Filter where = null, 
            bool truncate = false, 
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            string statement;
            if (where != null)
            {
                statement = @"
                    DELETE FROM " + tableName + @"
                    WHERE " + where;
                statement = statement.MultilineTrim();
            }
            else if (truncate)
            {
                statement = "TRUNCATE TABLE " + tableName;
            }
            else
            {
                statement = "DELETE FROM " + tableName;
            }
            peekStatement?.Invoke(statement);

            return Execute.SQL(conn, statement, timeout: timeout, modifyCommand: modifyCommand);
        }
    }
}
