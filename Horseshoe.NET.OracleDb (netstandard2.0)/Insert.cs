using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, DbPlatform.Oracle))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, platform: DbPlatform.Oracle))) + ")";

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            return Execute.SQL(conn, statement, timeout: timeout, modifyCommand: modifyCommand);
        }

        public static int Table
        (
            out int identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(out identity, conn, tableName, columns, getIdentitySql: getIdentitySql, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            out int identity,
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            string getIdentitySql = null,
            int? timeout = null,
            Action<OracleCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, DbPlatform.Oracle))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, DbPlatform.Oracle))) + @");
                " + (getIdentitySql ?? "SELECT " + SqlLiteral.Identity(DbPlatform.Oracle) + ";");

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            identity = (int)(Query.SQL.AsScalar(conn, statement, timeout: timeout, modifyCommand: modifyCommand) ?? throw new UtilityException("This INSERT did not produce an identity"));
            return 1;
        }
    }
}
