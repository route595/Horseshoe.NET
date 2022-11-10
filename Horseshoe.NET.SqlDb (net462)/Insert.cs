using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, DbPlatform.SqlServer))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, platform: DbPlatform.SqlServer))) + ")";

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            return Execute.SQL(conn, statement, timeout: timeout, modifyCommand: modifyCommand);
        }

        public static int Table
        (
            out int identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<SqlDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(out identity, conn, tableName, columns, getIdentitySql: getIdentitySql, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            out int identity,
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            string getIdentitySql = null,
            int? timeout = null,
            Action<SqlCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, DbPlatform.SqlServer))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, DbPlatform.SqlServer))) + @");
                " + (getIdentitySql ?? "SELECT " + SqlLiteral.Identity(DbPlatform.SqlServer) + ";");

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            identity = (int)(Query.SQL.AsScalar(conn, statement, timeout: timeout, modifyCommand: modifyCommand) ?? throw new UtilityException("This INSERT did not produce an identity"));
            return 1;
        }
    }
}
