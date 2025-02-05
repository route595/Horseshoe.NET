using System;
using System.Data.OleDb;

using Horseshoe.NET.Db;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Factory methods for deleting data rows and / or tables (because of varying syntaxes a concrete DB platform must be supplied).
    /// </summary>
    public static class Delete
    {
        private static string MessageRelayGroup => OleDbConstants.MessageRelayGroup;

        /// <summary>
        /// Creates a connection and deletes some or all of the rows in a table with option to drop.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of rows deleted.</returns>
        public static int Table
        (
            string tableName, 
            IFilter where,
            DbProvider provider = default,
            OleDbConnectionInfo connectionInfo = null, 
            bool drop = false,
            bool purge = false,
            int? commandTimeout = null,
            Action<OleDbConnection> peekConnection = null,
            Action<OleDbCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var rowsDeleted = Table(conn, tableName, where, provider: provider, drop: drop, purge: purge, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return rowsDeleted;
            }
        }

        /// <summary>
        /// Deletes some or all of the rows in a table with option to drop using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <returns>The number of rows deleted.</returns>
        public static int Table
        (
            OleDbConnection conn,
            string tableName, 
            IFilter where,
            DbProvider provider = default,
            OleDbTransaction transaction = null,
            bool drop = false, 
            bool purge = false, 
            int? commandTimeout = null,
            Action<OleDbCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildDeleteStatement(tableName, where, drop: drop, purge: purge, provider: provider);
            var result = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            SystemMessageRelay.RelayMethodReturn(returnDescription: (!(drop || purge) ? "rows deleted: " : "result: ") + result, group: MessageRelayGroup);
            return result;
        }
    }
}
