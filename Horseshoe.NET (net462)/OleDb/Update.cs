using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OleDb
{
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<DbParameter> columns, 
            OleDbConnectionInfo connectionInfo = null, 
            Filter where = null, 
            int? timeout = null,
            DbPlatform? platform = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> modifyCommand = null,
            Action<OleDbConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout, platform: platform, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OleDbConnection conn, 
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where = null, 
            int? timeout = null,
            DbPlatform? platform = null,
            Action<OleDbCommand> modifyCommand = null,
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
