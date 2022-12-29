﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
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
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<SqlCommand> alterCommand = null,
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
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, journal: journal))
            {
                var result = Table(conn, tableName, columns, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? commandTimeout = null,
            Action<SqlCommand> alterCommand = null,
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
            var statement = DbUtil.BuildInsertStatement(DbPlatform.SqlServer, tableName, columns, journal: journal);
            var result = Execute.SQL(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            string getIdentitySql = null,
            int? commandTimeout = null,
            Action<SqlCommand> alterCommand = null,
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
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, journal: journal))
            {
                var result = Table(out identity, conn, tableName, columns, getIdentitySql: getIdentitySql, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
        /// <param name="getIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            string getIdentitySql = null,
            int? commandTimeout = null,
            Action<SqlCommand> alterCommand = null,
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
            var statement = DbUtil.BuildInsertAndGetIdentityStatements(DbPlatform.SqlServer, tableName, columns, getIdentitySql: getIdentitySql, journal: journal);
            identity = Zap.NInt(Query.SQL.AsScalar(conn, statement, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal));

            // finalize
            journal.Level--;
            return 1;
        }
    }
}
