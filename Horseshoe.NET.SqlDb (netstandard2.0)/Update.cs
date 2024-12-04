using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.SqlDb
{
    /// <summary>
    /// Factory methods for updating database tables.
    /// </summary>
    public static class Update
    {
        /// <summary>
        /// Creates a connection and updates a database table.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            Filter where,
            SqlDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<SqlConnection> peekConnection = null,
            Action<SqlCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal))
            {
                var result = Table(conn, tableName, columns, where, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Updates a database table using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="transaction">An optional SQL transaction which bundles together multiple data calls over a single connection and commits or rolls back all of them</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            Filter where,
            SqlTransaction transaction = null,
            int? commandTimeout = null,
            Action<SqlCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildUpdateStatement(DbProvider.SqlServer, tableName, columns, where, journal: journal);
            var result = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
