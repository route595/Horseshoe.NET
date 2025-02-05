using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    /// <summary>
    /// The Odbc implemention of <see cref="QueryBase{ConnInfoT, ConnT, CmdT, DataAdapterT, XActionT}"/>
    /// </summary>
    public class Query : QueryBase<OdbcConnectionInfo, OdbcConnection, OdbcCommand, OdbcDataAdapter, OdbcTransaction>
    {
        private static string MessageRelayGroup => OdbcConstants.MessageRelayGroup;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="statement">A SQL statement or table/view/procedure/function name</param>
        /// <param name="commandType">The command type</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        public Query
        (
            OdbcConnectionInfo connectionInfo,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        : base
        (
            connectionInfo,
            statement,
            commandType,
            parameters: parameters,
            commandTimeout: commandTimeout,
            dbCapture: dbCapture,
            autoTrunc: autoTrunc,
            cryptoOptions: cryptoOptions,
            peekConnection: peekConnection,
            peekCommand: peekCommand
        )
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="statement">A SQL statement or table/view/procedure name.</param>
        /// <param name="commandType">The command type</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional DbCapture for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional <c>string</c> zapping hint</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        public Query
        (
            OdbcConnection connection,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        : base
        (
            connection,
            statement,
            commandType,
            parameters: parameters,
            commandTimeout: commandTimeout,
            transaction: transaction,
            dbCapture: dbCapture,
            autoTrunc: autoTrunc,
            peekCommand: peekCommand
        )
        { }

        /// <inheritdoc cref="OdbcUtil.BuildCommand(OdbcConnection, string, CommandType, IEnumerable{DbParameter}, OdbcTransaction, int?, Action{OdbcCommand})"/>
        public override OdbcCommand BuildCommand(OdbcConnection conn) =>
            OdbcUtil.BuildCommand(conn, Statement, CommandType, Parameters, Transaction, CommandTimeout, PeekCommand);

        /// <inheritdoc cref="OdbcUtil.LaunchConnection(OdbcConnectionInfo, Action{OdbcConnection}, CryptoOptions)"/>
        public override OdbcConnection LaunchConnection() =>
            OdbcUtil.LaunchConnection(connectionInfo: ConnectionInfo, peekConnection: PeekConnection, cryptoOptions: CryptoOptions);

        /// <summary>
        /// Subclasses need to implement this for DB output to <c>DataTable</c>
        /// </summary>
        /// <param name="cmd">A DB command</param>
        /// <returns>A DB data adapter</returns>
        public override OdbcDataAdapter BuildDataAdapter(OdbcCommand cmd) =>
            new OdbcDataAdapter(cmd);

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a table/view name
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="tableName">A table or view name</param>
        /// <param name="columnNames">The names of the columns to include in the query results, assume '*' if omitted</param>
        /// <param name="where">A filter which translates to a SQL <c>WHERE</c> clause</param>
        /// <param name="groupBy">One or more columns with which to add grouping to the query</param>
        /// <param name="orderBy">One or more columns by which to order the results</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions or entire SQL statements.</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableOrView
        (
            OdbcConnectionInfo connectionInfo,
            string tableName,
            StringValues columnNames = default,
            IFilter where = null,
            StringValues groupBy = default,
            StringValues orderBy = default,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connectionInfo,
                DbUtil.BuildSelectStatement(tableName, columnNames: columnNames, where: where, groupBy: groupBy, orderBy: orderBy, provider: provider),
                CommandType.Text,
                parameters: null,
                commandTimeout: commandTimeout,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                cryptoOptions: cryptoOptions,
                peekConnection: peekConnection,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance for a table/view
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="tableName">A table or view name</param>
        /// <param name="columnNames">The names of the columns to include in the query results, assume '*' if omitted</param>
        /// <param name="where">A filter which translates to a SQL <c>WHERE</c> clause</param>
        /// <param name="groupBy">One or more columns with which to add grouping to the query</param>
        /// <param name="orderBy">One or more columns by which to order the results</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions or entire SQL statements.</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableOrView
        (
            OdbcConnection connection,
            string tableName,
            StringValues columnNames = default,
            IFilter where = null,
            StringValues groupBy = default,
            StringValues orderBy = default,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connection,
                DbUtil.BuildSelectStatement(tableName, columnNames: columnNames, where: where, groupBy: groupBy, orderBy: orderBy, provider: provider),
                CommandType.Text,
                parameters: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a table/view name
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="tableName">A table or view name</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableOrViewDirect
        (
            OdbcConnectionInfo connectionInfo,
            string tableName,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connectionInfo,
                tableName,
                CommandType.TableDirect,
                parameters: null,
                commandTimeout: commandTimeout,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                cryptoOptions: cryptoOptions,
                peekConnection: peekConnection,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a table/view name
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="tableName">A table or view name</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableOrViewDirect
        (
            OdbcConnection connection,
            string tableName,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connection,
                tableName,
                CommandType.TableDirect,
                parameters: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a SQL statement
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="statement">A SQL statement</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromStatement
        (
            OdbcConnectionInfo connectionInfo,
            string statement,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connectionInfo,
                statement,
                CommandType.Text,
                parameters: parameters,
                commandTimeout: commandTimeout,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                cryptoOptions: cryptoOptions,
                peekConnection: peekConnection,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a SQL statement
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="statement">A SQL statement</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromStatement
        (
            OdbcConnection connection,
            string statement,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connection,
                statement,
                CommandType.Text,
                parameters: parameters,
                commandTimeout: commandTimeout,
                transaction: transaction,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a procedure name
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="procedureName">A stored procedure name</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional DbCapture for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional <c>string</c> zapping hint</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromStoredProcedure
        (
            OdbcConnectionInfo connectionInfo,
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connectionInfo,
                procedureName,
                CommandType.StoredProcedure,
                parameters: parameters,
                commandTimeout: commandTimeout,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                cryptoOptions: cryptoOptions,
                peekConnection: peekConnection,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a procedure name
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="procedureName">A stored procedure name</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional <c>DbCapture</c> for retrieving column metadata and out parameters</param>
        /// <param name="autoTrunc">An optional hint for zapping <c>string</c> data from the DB query</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromStoredProcedure
        (
            OdbcConnection connection,
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connection,
                procedureName,
                CommandType.StoredProcedure,
                parameters: parameters,
                commandTimeout: commandTimeout,
                transaction: transaction,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a function name
        /// </summary>
        /// <param name="connectionInfo">Info for creating an open DB connection</param>
        /// <param name="functionName">A function name</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="dbCapture">An optional DbCapture for retrieving column metadata and out parameters</param>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions or entire SQL statements.</param>
        /// <param name="autoTrunc">An optional <c>string</c> zapping hint</param>
        /// <param name="cryptoOptions">Optional cryptographic options for decrypting database passwords</param>
        /// <param name="peekConnection">Allows the caller access to the generated DB connection prior to command execution.</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableFunction
        (
            OdbcConnectionInfo connectionInfo,
            string functionName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OdbcConnection> peekConnection = null,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connectionInfo,
                DbUtil.BuildFunctionStatement(functionName, parameters: parameters, provider: provider),
                CommandType.Text,
                parameters: parameters,
                commandTimeout: commandTimeout,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                cryptoOptions: cryptoOptions,
                peekConnection: peekConnection,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }

        /// <summary>
        /// Factory method for creating a <c>QueryInfoBase</c> instance based off a function name
        /// </summary>
        /// <param name="connection">An open DB connection</param>
        /// <param name="functionName">A function name</param>
        /// <param name="parameters">Optional DB parameters</param>
        /// <param name="commandTimeout">An optional DB command timeout in seconds</param>
        /// <param name="transaction">An optional DB transaction</param>
        /// <param name="dbCapture">An optional DbCapture for retrieving column metadata and out parameters</param>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions or entire SQL statements.</param>
        /// <param name="autoTrunc">An optional <c>string</c> zapping hint</param>
        /// <param name="peekCommand">Allows the caller access to the DB command prior to execution.</param>
        /// <returns>A new <c>QueryInfo</c></returns>
        public static Query FromTableFunction
        (
            OdbcConnection connection,
            string functionName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OdbcTransaction transaction = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            Action<OdbcCommand> peekCommand = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var query = new Query
            (
                connection,
                DbUtil.BuildFunctionStatement(functionName, parameters: parameters, provider: provider),
                CommandType.Text,
                parameters: parameters,
                commandTimeout: commandTimeout,
                transaction: transaction,
                dbCapture: dbCapture,
                autoTrunc: autoTrunc,
                peekCommand: peekCommand
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return query;
        }
    }
}
