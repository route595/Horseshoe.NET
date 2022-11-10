﻿using System;
using System.Data.Odbc;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class Delete
    {
        public static int Table
        (
            string tableName, 
            OdbcConnectionInfo connectionInfo = null, 
            Filter where = null, 
            bool truncate = false,
            int? timeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OdbcCommand> modifyCommand = null,
            Action<OdbcConnectionInfo> resultantConnectionInfo = null,
            Action<string> peekStatement = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, resultantConnectionInfo: resultantConnectionInfo))
            {
                return Table(conn, tableName, where: where, truncate: truncate, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }
        }

        public static int Table
        (
            OdbcConnection conn, 
            string tableName, 
            Filter where = null, 
            bool truncate = false, 
            int? timeout = null,
            Action<OdbcCommand> modifyCommand = null,
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
