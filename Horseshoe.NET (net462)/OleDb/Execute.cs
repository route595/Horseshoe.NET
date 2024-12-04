using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Factory methods for executing non-query SQL statements.
    /// </summary>
    public static class Execute
    {
        /// <summary>
        /// Opens a connection and executes a non-query stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure being executed.</param>
        /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of affected rows.</returns>
        public static int Procedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            OleDbConnectionInfo connectionInfo = null, 
            DbCapture dbCapture = null,
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
                var result = Procedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Executes a non-query stored procedure on an open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="procedureName">The name of the stored procedure being executed.</param>
        /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of affected rows.</returns>
        public static int Procedure
        (
            OleDbConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null,
            OleDbTransaction transaction = null,
            DbCapture dbCapture = null,
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
            using (var cmd = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand))
            {
                var result = cmd.ExecuteNonQuery();

                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Opens a connection and executes a non-query SQL statement.
        /// </summary>
        /// <param name="statement">The SQL statement to execute.</param>
        /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of affected rows.</returns>
        public static int SQL
        (
            string statement,
            IEnumerable<DbParameter> parameters = null,
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
                var result = SQL(conn, statement, parameters: parameters, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);   // parameters optional here, e.g. may already be included in the SQL statement

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Executes a non-query SQL statement on an open connection.
        /// </summary>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="statement">The SQL statement to execute.</param>
        /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <returns>The number of affected rows.</returns>
        public static int SQL
        (
            OleDbConnection conn, 
            string statement,
            IEnumerable<DbParameter> parameters = null,
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
            using (var cmd = OleDbUtil.BuildTextCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                var result = cmd.ExecuteNonQuery();

                // finalize
                journal.Level--;
                return result;
            }
        }
    }
}
