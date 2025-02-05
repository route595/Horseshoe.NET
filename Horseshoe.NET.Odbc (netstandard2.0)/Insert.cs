using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;

using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Odbc
{
    /// <summary>
    /// Factory methods for inserting DB table rows.
    /// </summary>
    public static class Insert
    {
        private static string MessageRelayGroup => OdbcConstants.MessageRelayGroup;

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNamesAndValues">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columnNamesAndValues,
            OdbcConnectionInfo connectionInfo = null,
            DbProvider provider = default,
            int? commandTimeout = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var rowsInserted = Table(conn, tableName, columnNamesAndValues, provider: provider, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return rowsInserted;
            }
        }

        /// <summary>
        /// Inserts values into a table row using an existing open connection.
        /// </summary>
        /// <param name="conn">An open DB connection</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNamesAndValues">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            OdbcConnection conn,
            string tableName,
            IEnumerable<DbParameter> columnNamesAndValues,
            DbProvider provider = default,
            OdbcTransaction transaction = null,
            int? commandTimeout = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertStatement(tableName, columnNamesAndValues, provider: provider);
            var rowsInserted = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted: " + rowsInserted, group: MessageRelayGroup);
            return rowsInserted;
        }

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNamesAndValues">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="altGetIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            string tableName,
            IEnumerable<DbParameter> columnNamesAndValues,
            OdbcConnectionInfo connectionInfo = null,
            DbProvider provider = default,
            string altGetIdentitySql = null,
            int? commandTimeout = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
            {
                var rowsInserted = Table(out identity, conn, tableName, columnNamesAndValues, provider: provider, altGetIdentitySql: altGetIdentitySql, commandTimeout: commandTimeout, peekCommand: peekCommand);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return rowsInserted;
            }
        }

        /// <summary>
        /// Inserts values into a table row using an existing open connection.
        /// </summary>
        /// <param name="identity">The id of the inserted row.</param>
        /// <param name="conn">An open DB connection.</param>
        /// <param name="tableName">A table name.</param>
        /// <param name="columnNamesAndValues">The table columns and values to insert (uses <c>DbParameter</c> as column info).</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
        /// <param name="altGetIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            OdbcConnection conn,
            string tableName,
            IEnumerable<DbParameter> columnNamesAndValues,
            DbProvider provider = default,
            OdbcTransaction transaction = null,
            string altGetIdentitySql = null,
            int? commandTimeout = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertAndGetIdentityStatements(tableName, columnNamesAndValues, altGetIdentitySql: altGetIdentitySql, provider: provider);
            identity = Zap.NInt(Query.FromStatement(conn, statement, commandTimeout: commandTimeout, transaction: transaction, peekCommand: peekCommand).AsScalar());

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted = 1 (identity: " + ValueUtil.Display(identity) + ")", group: MessageRelayGroup);
            return 1;
        }
    }
}
