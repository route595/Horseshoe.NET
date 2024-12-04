using System;
using System.Data.SqlClient;
using System.Reflection;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.SqlDb
{
    /// <summary>
    /// Factory methods for deleting data rows and / or tables.
    /// </summary>
    public static class Delete
    {
        /// <summary>
        /// Creates a connection and deletes some or all of the rows in a table with option to drop.
        /// </summary>
        /// <param name="tableName">A table name</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Sql DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of rows deleted</returns>
        public static int Table
        (
            string tableName,
            Filter where,
            SqlDbConnectionInfo connectionInfo = null,
            bool drop = false,
            bool purge = false,
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
                var result = Table(conn, tableName, where, drop: drop, purge: purge, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Deletes some or all of the rows in a table with option to drop using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection</param>
        /// <param name="tableName">A table name</param>
        /// <param name="where">A filter indicating which rows to delete.</param>
        /// <param name="transaction">An optional SQL transaction which bundles together multiple data calls over a single connection and commits or rolls back all of them</param>
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Sql DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of rows deleted</returns>
        public static int Table
        (
            SqlConnection conn,
            string tableName,
            Filter where,
            SqlTransaction transaction = null,
            bool drop = false,
            bool purge = false,
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
            var statement = DbUtil.BuildDeleteStatement(DbProvider.SqlServer, tableName, where, drop: drop, purge: purge, journal: journal);
            var result = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
