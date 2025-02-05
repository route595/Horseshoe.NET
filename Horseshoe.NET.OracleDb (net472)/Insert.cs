using System;
using System.Collections.Generic;
using System.Data.Common;

using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    /// <summary>
    /// Factory methods for inserting DB table rows.
    /// </summary>
    public static class Insert
    {
        private static string MessageRelayGroup => OracleDbConstants.MessageRelayGroup;

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var result = Table(conn, tableName, columns, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleTransaction transaction = null,
            int? commandTimeout = null,
            Action<OracleCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertStatement(tableName, columns, provider: DbProvider.Oracle);
            var rowsInserted = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted: " + rowsInserted, group: MessageRelayGroup);
            return rowsInserted;
        }

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="altGetIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            OracleDbConnectionInfo connectionInfo = null,
            string altGetIdentitySql = null,
            int? commandTimeout = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var result = Table(out identity, conn, tableName, columns, altGetIdentitySql: altGetIdentitySql, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="altGetIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            OracleConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            string altGetIdentitySql = null,
            Action<OracleCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertAndGetIdentityStatements(tableName, columns, altGetIdentitySql: altGetIdentitySql, provider: DbProvider.Oracle);
            identity = Zap.NInt(Query.FromStatement(conn, statement, commandTimeout: commandTimeout, transaction: transaction, peekCommand: peekCommand).AsScalar());

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted = 1 (identity: " + ValueUtil.Display(identity) + ")", group: MessageRelayGroup);
            return 1;
        }
    }
}
