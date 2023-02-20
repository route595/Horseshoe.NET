using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Reflection;

using Horseshoe.NET.Crypto;
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
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            DbPlatform platform,
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where, 
            OleDbConnectionInfo connectionInfo = null, 
            int? commandTimeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> alterCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
            {
                var result = Table(conn, platform, tableName, columns, where, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Updates a database table using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to update (uses <c>DbParameter</c> as column info).</param>
        /// <param name="where">A filter indicating which rows to update.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of updated rows.</returns>
        public static int Table
        (
            OleDbConnection conn, 
            DbPlatform platform,
            string tableName, 
            IEnumerable<DbParameter> columns, 
            Filter where, 
            int? commandTimeout = null,
            Action<OleDbCommand> alterCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildUpdateStatement(platform, tableName, columns, where, journal: journal);
            var result = Execute.SQL(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand);

            // finalize
            journal.Level--;
            return result;
        }
    }
}
