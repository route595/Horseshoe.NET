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
    /// Factory methods for inserting DB table rows.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="cryptoOptions">Options for password decryption, if applicable</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            DbPlatform platform,
            string tableName,
            IEnumerable<DbParameter> columns,
            OleDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> alterCommand = null,
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
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
            {
                var result = Table(conn, platform, tableName, columns, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Inserts values into a table row using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection</param>
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            OleDbConnection conn,
            DbPlatform platform,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? commandTimeout = null,
            Action<OleDbCommand> alterCommand = null,
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
            var statement = DbUtil.BuildInsertStatement(platform, tableName, columns, journal: journal);
            var result = Execute.SQL(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

            // finalize
            journal.Level--;
            return result;
        }

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            DbPlatform platform,
            string tableName,
            IEnumerable<DbParameter> columns,
            OleDbConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? commandTimeout = null,
            CryptoOptions cryptoOptions = null,
            Action<OleDbCommand> alterCommand = null,
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
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
            {
                var result = Table(out identity, conn, platform, tableName, columns, getIdentitySql: getIdentitySql, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Inserts values into a table row using an existing open connection.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="platform">A DB platform lends hints about how to render SQL expression or entire SQL statements.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            OleDbConnection conn,
            DbPlatform platform,
            string tableName,
            IEnumerable<DbParameter> columns,
            string getIdentitySql = null,
            int? commandTimeout = null,
            Action<OleDbCommand> alterCommand = null,
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
            var statement = DbUtil.BuildInsertAndGetIdentityStatements(platform, tableName, columns, getIdentitySql: getIdentitySql, journal: journal); 
            identity = Zap.NInt(Query.SQL.AsScalar(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal));

            // finalize
            journal.Level--;
            return 1;
        }
    }
}
