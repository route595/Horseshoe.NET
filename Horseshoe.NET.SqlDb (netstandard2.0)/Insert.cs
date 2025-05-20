using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;

using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.SqlDb
{
    /// <summary>
    /// Factory methods for inserting DB table rows.
    /// </summary>
    public static class Insert
    {
        private static string MessageRelayGroup => SqlDbConstants.MessageRelayGroup;

        /// <summary>
        /// Creates a connection and inserts values into a table row.
        /// </summary>
        /// <param name="tableName">A table name.</param>
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info)</param>
        /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            int? commandTimeout = null,
            Action<SqlConnection> peekConnection = null,
            Action<SqlCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
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
        /// <param name="columns">The table columns and values to insert (uses <c>DbParameter</c> as column info)</param>
        /// <param name="transaction">An optional SQL transaction which bundles together multiple data calls over a single connection and commits or rolls back all of them</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlTransaction transaction = null,
            int? commandTimeout = null,
            Action<SqlCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertStatement(tableName, columns, provider: DbProvider.SqlServer);
            var rowsInserted = Execute.SQL(conn, statement, transaction: transaction, commandTimeout: commandTimeout, peekCommand: peekCommand);

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted: " + rowsInserted , group: MessageRelayGroup);
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
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlDbConnectionInfo connectionInfo = null,
            string altGetIdentitySql = null,
            int? commandTimeout = null,
            Action<SqlConnection> peekConnection = null,
            Action<SqlCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection))
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
        /// <param name="transaction">An optional SQL transaction which bundles together multiple data calls over a single connection and commits or rolls back all of them</param>
        /// <param name="altGetIdentitySql">An optional select statement for retrieving the identity of the inserted row.</param>
        /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error</param>
        /// <param name="peekCommand">Allows access to the underlying DB command for final inspection or alteration before executing</param>
        /// <returns>The number of inserted rows.</returns>
        public static int Table
        (
            out int? identity,
            SqlConnection conn,
            string tableName,
            IEnumerable<DbParameter> columns,
            SqlTransaction transaction = null,
            string altGetIdentitySql = null,
            int? commandTimeout = null,
            Action<SqlCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var statement = DbUtil.BuildInsertAndGetIdentityStatements(tableName, columns, altGetIdentitySql: altGetIdentitySql, provider: DbProvider.SqlServer);
            identity = Zap.NInt(Query.FromStatement(conn, statement, commandTimeout: commandTimeout, transaction: transaction, peekCommand: peekCommand).AsScalar());

            SystemMessageRelay.RelayMethodReturn(returnDescription: "rows inserted = 1 (identity: " + ValueUtil.Display(identity) + ")", group: MessageRelayGroup);
            return 1;
        }
    }
}
