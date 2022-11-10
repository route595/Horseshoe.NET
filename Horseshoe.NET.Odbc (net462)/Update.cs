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
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<DbParameter> columns, 
            OdbcConnectionInfo connectionInfo = null, 
            Filter where = null, 
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
                return Table(conn, tableName, columns, where: where, timeout: timeout, platform: platform, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OdbcConnection conn, 
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where = null, 
            int? timeout = null,
            DbPlatform? platform = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<string> peekStatement = null
        )
        {
            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString(platform: platform)));
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
