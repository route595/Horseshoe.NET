using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Odbc
{
    /// <summary>
    /// Factory methods for executing non-query SQL statements.
    /// </summary>
    public static class Execute
    {
        private static string MessageRelayGroup => OdbcConstants.MessageRelayGroup;

        /// <summary>
        /// Opens a connection and executes a non-query stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure being executed.</param>
        /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of affected rows.</returns>
        public static int Procedure
        (
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            OdbcConnectionInfo connectionInfo = null,
            DbCapture dbCapture = null,
            int? commandTimeout = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var result = Procedure(conn, procedureName, parameters: parameters, dbCapture: dbCapture, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of affected rows.</returns>
        public static int Procedure
        (
            OdbcConnection conn,
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            int? commandTimeout = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var cmd = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand))
            {
                var rowsAffected = cmd.ExecuteNonQuery();

                if (dbCapture != null)
                {
                    dbCapture.OutputParameters = cmd.Parameters
                        .Cast<OdbcParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                SystemMessageRelay.RelayMethodReturn(returnDescription: "rows affected: " + rowsAffected, group: MessageRelayGroup);
                return rowsAffected;
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
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of affected rows.</returns>
        public static int SQL
        (
            string statement,
            IEnumerable<DbParameter> parameters = null,
            OdbcConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var rowsAffected = SQL(conn, statement, parameters: parameters, commandTimeout: commandTimeout, peekCommand: peekCommand);   // parameters optional here, e.g. may already be included in the SQL statement

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return rowsAffected;
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
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of affected rows.</returns>
        public static int SQL
        (
            OdbcConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters = null,
            OdbcTransaction transaction = null,
            int? commandTimeout = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var cmd = OdbcUtil.BuildTextCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                var rowsAffected = cmd.ExecuteNonQuery();

                SystemMessageRelay.RelayMethodReturn(returnDescription: "rows affected: " + rowsAffected, group: MessageRelayGroup);
                return rowsAffected;
            }
        }
    }
}
