using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            OdbcConnectionInfo connectionInfo = null,
            int? timeout = null,
            DbPlatform? platform = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, timeout: timeout, platform: platform, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OdbcConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? timeout = null,
            DbPlatform? platform = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, platform: platform))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, platform: platform))) + ")";

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            return Execute.SQL(conn, statement, timeout: timeout, modifyCommand: modifyCommand);
        }

        public static int Table
        (
            out int identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            OdbcConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? timeout = null,
            DbPlatform? platform = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(out identity, conn, tableName, columns, getIdentitySql: getIdentitySql, timeout: timeout, platform: platform, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            out int identity,
            OdbcConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            string getIdentitySql = null,
            int? timeout = null,
            DbPlatform? platform = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => DbUtil.RenderColumnName(c, platform: platform))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DbUtil.Sqlize(c.Value, platform: platform))) + @");
                " + (getIdentitySql ?? "SELECT " + SqlLiteral.Identity(platform ?? default) + ";");

            statement = statement.MultilineTrim();
            peekStatement?.Invoke(statement);

            identity = (int)(Query.SQL.AsScalar(conn, statement, timeout: timeout, modifyCommand: modifyCommand) ?? throw new UtilityException("This INSERT did not produce an identity"));
            return 1;
        }
    }
}
