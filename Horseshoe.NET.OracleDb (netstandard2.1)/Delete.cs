using System;
using System.Reflection;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
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
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of rows deleted</returns>
        public static int Table
        (
            string tableName,
            Filter where,
            OracleDbConnectionInfo connectionInfo = null,
            bool drop = false,
            bool purge = false,
            int? commandTimeout = null,
            Action<OracleCommand> alterCommand = null,
            TraceJournal journal = null
        )
        {
            // trace journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, journal: journal))
            {
                var result = Table(conn, tableName, where, drop: drop, purge: purge, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
        /// <param name="drop">If <c>true</c>, deletes the table database object (rather than just delete rows), default is <c>false</c>.</param>
        /// <param name="purge">Oracle DB only. If <c>true</c> and if <c>drop == true</c>, deletes the table database object (rather than just delete rows) and releases the space associated with it in a single step, default is <c>false</c>.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of rows deleted</returns>
        public static int Table
        (
            OracleConnection conn,
            string tableName,
            Filter where,
            bool drop = false,
            bool purge = false,
            int? commandTimeout = null,
            Action<OracleCommand> alterCommand = null,
            TraceJournal journal = null
        )
        {
            // trace journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildDeleteStatement(DbPlatform.Oracle, tableName, where, drop: drop, purge: purge, journal: journal);
            var result = Execute.SQL(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
