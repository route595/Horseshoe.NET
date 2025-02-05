using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;

using Horseshoe.NET.Db;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Factory methods for updating database tables.
    /// </summary>
    public static class Update
    {
        private static string MessageRelayGroup => OleDbConstants.MessageRelayGroup;

        /// <summary>
        /// Creates a connection and updates a database table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            string tableName, 
            IEnumerable<DbParameter> columns, 
            IFilter where, 
            OleDbConnectionInfo connectionInfo = null, 
            DbProvider provider = default,
            int? commandTimeout = null,
            Action<OleDbConnection> peekConnection = null,
            Action<OleDbCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var rowsUpdated = Table(conn, tableName, columns, where, provider: provider, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return rowsUpdated;
            }
        }

        /// <summary>
        /// Updates a database table using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            OleDbConnection conn, 
            string tableName, 
            IEnumerable<DbParameter> columns, 
            IFilter where,
            DbProvider provider = default,
            OleDbTransaction transaction = null,
            int? commandTimeout = null,
            Action<OleDbCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildUpdateStatement(tableName, columns, where, provider: provider);
            var rowsUpdated = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows updated: " + rowsUpdated, group: MessageRelayGroup);
            return rowsUpdated;
        }
    }
}
