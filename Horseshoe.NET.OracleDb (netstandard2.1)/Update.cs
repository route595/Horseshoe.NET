using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

using Horseshoe.NET.Db;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
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
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            Filter where,
            OracleDbConnectionInfo connectionInfo = null,
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
                var result = Table(conn, tableName, columns, where, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            Filter where,
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
            var statement = DbUtil.BuildUpdateStatement(DbPlatform.Oracle, tableName, columns, where, journal: journal);
            var result = Execute.SQL(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
