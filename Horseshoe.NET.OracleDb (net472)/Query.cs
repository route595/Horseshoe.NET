using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    /// <summary>
    /// The Oracle implemention of <see cref="QueryBase{ConnInfoT, ConnT, CmdT, DataAdapterT, XActionT}"/>
    /// </summary>
    public class Query : QueryBase<OracleDbConnectionInfo, OracleConnection, OracleCommand, OracleDataAdapter, OracleTransaction>
    {
        private static string MessageRelayGroup => OracleDbConstants.MessageRelayGroup;

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
            OracleDbConnectionInfo connectionInfo,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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

        /// <inheritdoc cref="OracleUtil.BuildCommand(OracleConnection, string, CommandType, IEnumerable{DbParameter}, OracleTransaction, int?, Action{OracleCommand})"/>
        public override OracleCommand BuildCommand(OracleConnection conn) =>
            OracleDbUtil.BuildCommand(conn, Statement, CommandType, Parameters, Transaction, CommandTimeout, PeekCommand);

        /// <inheritdoc cref="OracleUtil.LaunchConnection(OracleConnectionInfo, Action{OracleConnection}, CryptoOptions)"/>
        public override OracleConnection LaunchConnection() =>
            OracleDbUtil.LaunchConnection(connectionInfo: ConnectionInfo, peekConnection: PeekConnection, cryptoOptions: CryptoOptions);

        /// <summary>
        /// Subclasses need to implement this for DB output to <c>DataTable</c>
        /// </summary>
        /// <param name="cmd">A DB command</param>
        /// <returns>A DB data adapter</returns>
        public override OracleDataAdapter BuildDataAdapter(OracleCommand cmd) =>
            new OracleDataAdapter(cmd);

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
            OracleDbConnectionInfo connectionInfo,
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
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string tableName,
            StringValues columnNames = default,
            IFilter where = null,
            StringValues groupBy = default,
            StringValues orderBy = default,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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
            OracleDbConnectionInfo connectionInfo,
            string tableName,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string tableName,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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
            OracleDbConnectionInfo connectionInfo,
            string statement,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string statement,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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
            OracleDbConnectionInfo connectionInfo,
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string procedureName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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
            OracleDbConnectionInfo connectionInfo,
            string functionName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<OracleConnection> peekConnection = null,
            Action<OracleCommand> peekCommand = null
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
            OracleConnection connection,
            string functionName,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            OracleTransaction transaction = null,
            DbCapture dbCapture = null,
            DbProvider provider = default,
            AutoTruncate autoTrunc = default,
            Action<OracleCommand> peekCommand = null
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
