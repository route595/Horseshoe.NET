using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Dotnet;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Query info upon which to build SQL connections and handle query results
    /// </summary>
    public abstract class QueryBase<ConnInfoT, ConnT, CmdT, DataAdapterT, XActionT>
        where ConnInfoT : ConnectionInfo
        where ConnT : DbConnection
        where CmdT : DbCommand
        where DataAdapterT : DataAdapter
        where XActionT : DbTransaction
    {
        private static string MessageRelayGroup => DbConstants.MessageRelayGroup;

        /// <summary>
        /// Info for creating an open DB connection
        /// </summary>
        public ConnInfoT ConnectionInfo { get; }

        /// <summary>
        /// An open DB connection
        /// </summary>
        public ConnT Connection { get; }

        /// <summary>
        /// A SQL statement or table/view/procedure/function name
        /// </summary>
        public string Statement { get; }

        /// <summary>
        /// The command type
        /// </summary>
        public CommandType CommandType { get; }

        /// <summary>
        /// Optional DB parameters
        /// </summary>
        public IEnumerable<DbParameter> Parameters { get; }

        /// <summary>
        /// An optional DB command timeout in seconds
        /// </summary>
        public int? CommandTimeout { get; }

        /// <summary>
        /// An optional DB transaction
        /// </summary>
        public XActionT Transaction { get; }

        /// <summary>
        /// An optional <c>DbCapture</c> for retrieving column metadata and out parameters
        /// </summary>
        public DbCapture DbCapture { get; }

        /// <summary>
        /// An optional hint for zapping <c>string</c> data from the DB query
        /// </summary>
        public AutoTruncate AutoTrunc { get; }

        /// <summary>
        /// Optional cryptographic options for decrypting database passwords
        /// </summary>
        public CryptoOptions CryptoOptions { get; }

        /// <summary>
        /// Allows the caller access to the generated DB connection prior to command execution.
        /// </summary>
        public Action<ConnT> PeekConnection { get; }

        /// <summary>
        /// Allows the caller access to the DB command prior to execution.
        /// </summary>
        public Action<CmdT> PeekCommand { get; }

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
        public QueryBase
        (
            ConnInfoT connectionInfo,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            CryptoOptions cryptoOptions = null,
            Action<ConnT> peekConnection = null,
            Action<CmdT> peekCommand = null
        )
        {
            ConnectionInfo = connectionInfo;
            Statement = statement;
            CommandType = commandType;
            Parameters = parameters;
            CommandTimeout = commandTimeout;
            DbCapture = dbCapture;
            AutoTrunc = autoTrunc;
            CryptoOptions = cryptoOptions;
            PeekConnection = peekConnection;
            PeekCommand = peekCommand;
        }

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
        public QueryBase
        (
            ConnT connection,
            string statement,
            CommandType commandType,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            XActionT transaction = null,
            DbCapture dbCapture = null,
            AutoTruncate autoTrunc = default,
            Action<CmdT> peekCommand = null
        )
        {
            Connection = connection;
            Statement = statement;
            CommandType = commandType;
            Parameters = parameters;
            CommandTimeout = commandTimeout;
            Transaction = transaction;
            DbCapture = dbCapture;
            AutoTrunc = autoTrunc;
            PeekCommand = peekCommand;
        }

        /// <summary>
        /// Subclasses must implement this for supplying open database connections to all <c>AsXyz()</c> methods.
        /// <code>
        /// // OleDb Example
        /// public override OleDbConnection LaunchConnection() =>
        ///     OleDbUtil.LaunchConnection("DSN=MySqlServer;DB=MyDatabase;UID=sam.elliot;PWD=%*y3b!)0")
        /// </code>
        /// </summary>
        /// <returns>An open database connection.</returns>
        public abstract ConnT LaunchConnection();

        /// <summary>
        /// Subclasses must implement this for supplying database commands to all <c>AsXyz()</c> methods.
        /// <code>
        /// // OleDb Example
        /// public override OleDbCommand BuildCommand(OleDbConnection conn) =>
        ///     OleDbUtil.BuildCommand(conn, CommandType.TableDirect, "[dbo].[Employees]")
        /// </code>
        /// </summary>
        /// <param name="conn">An open database connection</param>
        /// <returns>A database command based on the properties of the current <c>QueryBased</c> and the supplied database connection</returns>
        public abstract CmdT BuildCommand(ConnT conn);

        /// <summary>
        /// Subclasses need to implement this for DB output to <c>DataTable</c>
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public abstract DataAdapterT BuildDataAdapter(CmdT cmd);

        /// <summary>
        /// Executes the database query defined in this <c>QueryBase</c> and returns the rows as objects of type <c>T</c>.
        /// By convention, the properties of <c>T</c> correspond to the data columns returned by the query.
        /// </summary>
        /// <returns>A collection of type <c>T</c></returns>
        public IEnumerable<T> AsCollection<T>(RowParser<T> rowParser = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var list = AsList(Connection, rowParser: rowParser);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
            using (var conn = LaunchConnection())
            {
                var list = AsList(conn, rowParser: rowParser);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Asynchronously executes the database query defined in this <c>QueryBase</c> and returns the rows as objects of type <c>T</c>.
        /// By convention, the properties of <c>T</c> correspond to the data columns returned by the query.
        /// </summary>
        /// <returns>A collection of type <c>T</c></returns>
        public async Task<IEnumerable<T>> AsCollectionAsync<T>(RowParser<T> rowParser = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var list = await AsListAsync(Connection, rowParser: rowParser);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
            using (var conn = LaunchConnection())
            {
                var list = await AsListAsync(conn, rowParser: rowParser);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Executes the database query defined in this <c>QueryBase</c> and returns the rows as arrays of <c>object</c>s. 
        /// </summary>
        /// <returns>An <c>object[]</c></returns>
        public IEnumerable<object[]> AsObjects<T>()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var list = AsObjects(Connection);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
            using (var conn = LaunchConnection())
            {
                var list = AsObjects(conn);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Asynchronously executes the database query defined in this <c>QueryBase</c> and returns the rows as arrays of <c>object</c>s. 
        /// </summary>
        /// <returns>An <c>object[]</c></returns>
        public async Task<IEnumerable<object[]>> AsObjectsAsync()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var list = await AsObjectsAsync(Connection);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
            using (var conn = LaunchConnection())
            {
                var list = await AsObjectsAsync(conn);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return list;
            }
        }

        /// <summary>
        /// Executes the database query defined in this <c>QueryBase</c> and returns the data in a <c>DataTable</c>. 
        /// </summary>
        /// <returns>A <c>DataTable</c></returns>
        public DataTable AsDataTable()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var dataSet = AsDataSet(Connection, CommandType.In(CommandType.StoredProcedure, CommandType.TableDirect) ? Statement : null);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
            }
            using (var conn = LaunchConnection())
            {
                var dataSet = AsDataSet(conn, CommandType.In(CommandType.StoredProcedure, CommandType.TableDirect) ? Statement : null);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
            }
        }

        /// <summary>
        /// Executes the database query defined in this <c>QueryBase</c> and exposes the raw data reader. 
        /// </summary>
        /// <param name="keepOpen">If <c>true</c>, keeps the connec</param>
        /// <returns>The <c>IDataReader</c></returns>
        public IDataReader AsDataReader(bool keepOpen = false)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var dataReader = AsDataReader(Connection, keepOpen);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataReader;
            }
            using (var conn = LaunchConnection())
            {
                var dataReader = AsDataReader(conn, keepOpen);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataReader;
            }
        }

        /// <summary>
        /// Asynchronously executes the database query defined in this <c>QueryBase</c> and exposes the raw data reader. 
        /// </summary>
        /// <param name="keepOpen">If <c>true</c>, keeps the connec</param>
        /// <returns>The <c>IDataReader</c></returns>
        public async Task<IDataReader> AsDataReaderAsync(bool keepOpen = false)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var dataReader = await AsDataReaderAsync(Connection, keepOpen);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataReader;
            }
            using (var conn = LaunchConnection())
            {
                var dataReader = await AsDataReaderAsync(conn, keepOpen);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return dataReader;
            }
        }

        /// <summary>
        /// Executes the database query defined in this <c>QueryBase</c> and returns a scalar value. 
        /// </summary>
        /// <returns>A scalar value</returns>
        public object AsScalar()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var obj = AsScalar(Connection);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return obj;
            }
            using (var conn = LaunchConnection())
            {
                var obj = AsScalar(conn);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return obj;
            }
        }

        /// <summary>
        /// Asynchronously executes the database query defined in this <c>QueryBase</c> and returns a scalar value. 
        /// </summary>
        /// <returns>A scalar value</returns>
        public async Task<object> AsScalarAsync()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (Connection != null)
            {
                var obj = await AsScalarAsync(Connection);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return obj;
            }
            using (var conn = LaunchConnection())
            {
                var obj = await AsScalarAsync(conn);
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return obj;
            }
        }

        private IEnumerable<T> AsList<T>(ConnT conn, RowParser<T> rowParser = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            rowParser = rowParser ?? RowParser.BuildAutoParser<T>();  // column -> property mapper
            var list = new List<T>();
            using (var command = BuildCommand(conn))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    if (DbCapture != null)
                    {
                        DbCapture.DataColumns = dataReader.GetDataColumns();
                        DbCapture.OutputParameters = command.Parameters
                            .Cast<DbParameter>()
                            .Where(p => p.Direction == ParameterDirection.Output)
                            .ToArray();
                    }

                    if (rowParser.IsReaderParser)
                    {
                        while (dataReader.Read())
                        {
                            list.Add(rowParser.Parse(dataReader));
                        }
                    }
                    else if (rowParser.IsObjectParser)
                    {
                        var objectArrays = DbUtil.ReadAsObjects(dataReader, dbCapture: null, autoTrunc: AutoTrunc);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                    else
                    {
                        throw new ThisShouldNeverHappenException("No row parser.");
                    }
                }
            }
            SystemMessageRelay.RelayMethodReturn(returnDescription: DotnetUtil.GetSourceCodeFormattedName(typeof(T)) + " count: " + list.Count, group: MessageRelayGroup);
            return list;
        }

        private async Task<IEnumerable<T>> AsListAsync<T>(ConnT conn, RowParser<T> rowParser = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            rowParser = rowParser ?? RowParser.BuildAutoParser<T>();  // column -> property mapper
            var list = new List<T>();
            using (var command = BuildCommand(conn))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    if (rowParser.IsReaderParser)
                    {
                        if (DbCapture != null)
                        {
                            DbCapture.DataColumns = dataReader.GetDataColumns();
                            DbCapture.OutputParameters = command.Parameters
                                .Cast<DbParameter>()
                                .Where(p => p.Direction == ParameterDirection.Output)
                                .ToArray();
                        }

                        while (await dataReader.ReadAsync())
                        {
                            list.Add(rowParser.Parse(dataReader));
                        }
                    }
                    else if (rowParser.IsObjectParser)
                    {
                        var objectArrays = await DbUtil.ReadAsObjectsAsync(dataReader, dbCapture: null, autoTrunc: AutoTrunc);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                    else
                    {
                        throw new ThisShouldNeverHappenException("No row parser.");
                    }
                }
            }
            SystemMessageRelay.RelayMethodReturn(returnDescription: DotnetUtil.GetSourceCodeFormattedName(typeof(T)) + " count: " + list.Count, group: MessageRelayGroup);
            return list;
        }

        private IEnumerable<object[]> AsObjects(ConnT conn)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var command = BuildCommand(conn))
            {
                var list = DbUtil.ReadAsObjects(command, dbCapture: DbCapture, autoTrunc: AutoTrunc);

                SystemMessageRelay.RelayMethodReturn(returnDescription: "object[] count: " + list.Count, group: MessageRelayGroup);
                return list;
            }
        }

        private async Task<IEnumerable<object[]>> AsObjectsAsync(ConnT conn)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var command = BuildCommand(conn))
            {
                var list = await DbUtil.ReadAsObjectsAsync(command, dbCapture: DbCapture, autoTrunc: AutoTrunc);

                SystemMessageRelay.RelayMethodReturn(returnDescription: "object[] count: " + list.Count, group: MessageRelayGroup);
                return list;
            }
        }

        private DataSet AsDataSet(ConnT conn, string tableName)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var command = BuildCommand(conn))
            {
                using (var adapter = BuildDataAdapter(command))
                {
                    var dataSet = new DataSet();
                    SystemMessageRelay.RelayMessage("adapter.Fill()", group: MessageRelayGroup);
                    adapter.Fill(dataSet);
                    if (dataSet.Tables.Count > 0)
                    {
                        if (tableName != null)
                            dataSet.Tables[0].TableName = tableName;
                        SystemMessageRelay.RelayMethodReturn(returnDescription: "tables[0] row count: " + dataSet.Tables[0].Rows.Count, group: MessageRelayGroup);
                    }
                    else
                    {
                        SystemMessageRelay.RelayMethodReturn(returnDescription: "no tables", group: MessageRelayGroup);
                    }
                    return dataSet;
                }
            }
        }

        private IDataReader AsDataReader(ConnT conn, bool keepOpen)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var command = BuildCommand(conn);
            var dataReader = command.ExecuteReader(keepOpen ? CommandBehavior.Default : CommandBehavior.CloseConnection);

            if (DbCapture != null)
            {
                DbCapture.DataColumns = dataReader.GetDataColumns();
                DbCapture.OutputParameters = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return dataReader;
        }

        private async Task<IDataReader> AsDataReaderAsync(ConnT conn, bool keepOpen)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var command = BuildCommand(conn);
            var dataReader = await command.ExecuteReaderAsync(keepOpen ? CommandBehavior.Default : CommandBehavior.CloseConnection);

            if (DbCapture != null)
            {
                DbCapture.DataColumns = dataReader.GetDataColumns();
                DbCapture.OutputParameters = command.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output)
                    .ToArray();
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return dataReader;
        }

        private object AsScalar(ConnT conn)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var command = BuildCommand(conn))
            {
                var result = command.ExecuteScalar();
                result = DbUtil.ProcessScalarResult(result, autoTrunc: AutoTrunc);
                SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
                return result;
            }
        }

        private async Task<object> AsScalarAsync(ConnT conn)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            using (var command = BuildCommand(conn))
            {
                var result = await command.ExecuteScalarAsync();
                result = DbUtil.ProcessScalarResult(result, autoTrunc: AutoTrunc);
                SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
                return result;
            }
        }
    }
}
