using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

using Horseshoe.NET.Db;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    /// <summary>
    /// Factory methods for inserting DB table rows.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal))
            {
                var result = Table(conn, tableName, columns, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Inserts values into a table row using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleTransaction transaction = null,
            int? commandTimeout = null,
            Action<OracleCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildInsertStatement(DbProvider.Oracle, tableName, columns, journal: journal);
            var result = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

            // finalize
            journal.Level--;
            return result;
        }

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? commandTimeout = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal))
            {
                var result = Table(out identity, conn, tableName, columns, getIdentitySql: getIdentitySql, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

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
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <param name="journal">A trace journal to which each step of the process is logged</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleTransaction transaction = null,
            string getIdentitySql = null,
            int? commandTimeout = null,
            Action<OracleCommand> peekCommand = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteMethodDisplayName(MethodBase.GetCurrentMethod());
            journal.Level++;

            // data stuff
            var statement = DbUtil.BuildInsertAndGetIdentityStatements(DbProvider.Oracle, tableName, columns, getIdentitySql: getIdentitySql, journal: journal);
            identity = Zap.NInt(Query.SQL.AsScalar(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal));

            // finalize
            journal.Level--;
            return 1;
        }
    }
}
