using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Reflection;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Factory methods for updating database tables.
    /// </summary>
    public static class Update
    {
        /// <summary>
        /// Creates a connection and updates a database table.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            DbProvider provider,
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where, 
            OleDbConnectionInfo connectionInfo = null, 
            int? commandTimeout = null,
            Action<OleDbConnection> peekConnection = null,
            Action<OleDbCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal))
            {
                var result = Table(conn, provider, tableName, columns, where, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Updates a database table using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            OleDbConnection conn, 
            DbProvider provider,
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where,
            OleDbTransaction transaction = null,
            int? commandTimeout = null,
            Action<OleDbCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildUpdateStatement(provider, tableName, columns, where, journal: journal);
            var result = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
