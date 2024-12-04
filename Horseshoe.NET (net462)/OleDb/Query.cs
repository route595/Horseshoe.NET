using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// Factory methods for querying database objects
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// Factory methods for executing SQL query statements
        /// </summary>
        public static class SQL
        {
            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsCollection(conn, statement, dbCapture: dbCapture, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsCollectionAsync(conn, statement, dbCapture: dbCapture, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var result = BuildList
                (
                    conn,
                    statement,
                    null,
                    transaction,
                    dbCapture,
                    rowParser,
                    sorter, 
                    autoTrunc,
                    commandTimeout, 
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var result = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    transaction,
                    dbCapture,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsObjects(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsObjectsAsync(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = AsDataReader(conn, statement, transaction: transaction, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = await AsDataReaderAsync(conn, statement, transaction: transaction, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query. 
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsScalar(conn, statement, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsScalarAsync(conn, statement, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                AutoTruncate autoTrunc = default,
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
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, transaction, commandTimeout, peekCommand))
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                AutoTruncate autoTrunc = default,
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
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, transaction, commandTimeout, peekCommand))
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string statement,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = await AsDataReaderAsync(conn, statement, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var command = OleDbUtil.BuildTextCommand(conn, statement, null, transaction, commandTimeout, peekCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OleDbConnection conn,
                string statement,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var command = OleDbUtil.BuildTextCommand(conn, statement, null, transaction, commandTimeout, peekCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes the user-supplied SQL query.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="tableName">An optional name to assign to the <c>DataTable</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string statement,
                string tableName = null,
                string tableNamespace = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsDataTable(conn, statement, tableName: tableName, tableNamespace: tableNamespace, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="tableName">An optional name to assign to the <c>DataTable</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="ValidationException"></exception>
            public static DataTable AsDataTable
            (
                OleDbConnection conn,
                string statement,
                string tableName = null,
                string tableNamespace = null,
                OleDbTransaction transaction = null,
                AutoTruncate autoTrunc = default,
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
                DataTable result;
                if (tableName != null)
                {
                    if (tableNamespace != null)
                    {
                        result = new DataTable(tableName, tableNamespace);
                    }
                    else
                    {
                        result = new DataTable(tableName);
                    }
                }
                else
                {
                    result = new DataTable();
                }
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, transaction, commandTimeout, peekCommand))
                {
                    using (var adapter = new OleDbDataAdapter(command))
                    {
                        journal.WriteEntry("adapter.Fill()");
                        adapter.Fill(result);
                    }
                }

                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    throw new ValidationException("Zap does not work on data tables");
                if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    DbUtil.TrimDataTable(result);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Factory methods for executing table or view based queries
        /// </summary>
        public static class Table
        {
            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsCollection(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsCollectionAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OleDbConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var statement = BuildStatement
                (
                    tableName, 
                    columns, 
                    where, 
                    groupBy, 
                    orderBy, 
                    journal
                );

                var result = BuildList
                (
                    conn,
                    statement,
                    null,
                    null,
                    dbCapture,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OleDbConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                var result = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    null,
                    dbCapture,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsObjects(conn, tableName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsObjectsAsync(conn, tableName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OleDbConnection conn,
                string tableName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = AsDataReader(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OleDbConnection conn,
                string tableName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = await AsDataReaderAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="column">The column in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsScalar(conn, tableName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="column">The column in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsScalarAsync(conn, tableName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, commandTimeout: commandTimeout, autoTrunc: autoTrunc, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="column">The column in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OleDbConnection conn,
                string tableName,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                var statement = BuildStatement
                (
                    tableName,
                    column != null ? new[] { column } : null,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="column">The column in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OleDbConnection conn,
                string tableName,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                var statement = BuildStatement
                (
                    tableName,
                    column != null ? new[] { column } : null,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = AsDataReader(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string tableName,
                OleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = await AsDataReaderAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OleDbConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OleDbConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied table or view.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="tableName">A table or view to query.</param>
            /// <param name="altTableName">An optional name to assign to the <c>DataTable</c>, default is <c>tableName</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string tableName,
                string altTableName = null,
                string tableNamespace = null,
                OleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsDataTable(conn, tableName, altTableName: altTableName, tableNamespace: tableNamespace, columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied table or view using an existing open connection.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="altTableName">An optional name to assign to the <c>DataTable</c>, default is <c>tableName</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="ValidationException"></exception>
            public static DataTable AsDataTable
            (
                OleDbConnection conn,
                string tableName,
                string altTableName = null,
                string tableNamespace = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
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
                DataTable result;
                if (tableNamespace != null)
                {
                    result = new DataTable(altTableName ?? tableName, tableNamespace);
                }
                else
                {
                    result = new DataTable(altTableName ?? tableName);
                }
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))
                {
                    using (var adapter = new OleDbDataAdapter(command))
                    {
                        journal.WriteEntry("adapter.Fill()");
                        adapter.Fill(result);
                    }
                }

                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    throw new ValidationException("Zap does not work on data tables");
                if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    DbUtil.TrimDataTable(result);

                // finalize
                journal.Level--;
                return result;
            }

            private static string BuildStatement
            (
                string tableName,
                IEnumerable<string> columns,
                IFilter where,
                IEnumerable<string> groupBy,
                IEnumerable<string> orderBy,
                TraceJournal journal
            )
            {
                // journaling
                journal.WriteEntry("BuildStatement()");
                journal.Level++;

                // data stuff
                var statement = @"
                    SELECT " + (columns != null ? string.Join(", ", columns) : "*") + @"
                    FROM " + tableName;
                if (where != null)
                {
                    statement += @"
                    WHERE " + where.Render();
                }
                if (groupBy != null)
                {
                    statement += @"
                    GROUP BY " + string.Join(", ", groupBy);
                }
                if (orderBy != null)
                {
                    statement += @"
                    ORDER BY " + string.Join(", ", orderBy);
                }

                statement = statement.MultilineTrim();
                journal.AddAndWriteEntry("sql.statement", string.Join(" ", statement.Replace("\r\n", "\n").Split('\n')));

                // finalize
                journal.Level--;
                return statement;
            }
        }

        /// <summary>
        /// Factory methods for executing stored procedures
        /// </summary>
        public static class Procedure
        {
            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsCollection<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsCollectionAsync<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var result = BuildList
                (
                    conn,
                    procedureName,
                    parameters ?? Enumerable.Empty<DbParameter>(),
                    transaction,
                    dbCapture,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var result = await BuildListAsync
                (
                    conn,
                    procedureName,
                    parameters ?? Enumerable.Empty<DbParameter>(),
                    transaction,
                    dbCapture,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsObjects(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsObjectsAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, transaction: transaction, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = await AsDataReaderAsync(conn, procedureName, parameters: parameters, transaction: transaction, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsScalar(conn, procedureName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsScalarAsync(conn, procedureName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                AutoTruncate autoTrunc = default,
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
                using (var command = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand))
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                AutoTruncate autoTrunc = default,
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
                using (var command = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand))
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var command = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OleDbConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var command = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied stored procedure.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="tableName">An optional name to assign to the <c>DataTable</c>, default is <c>procedureName</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string procedureName,
                string tableName = null,
                string tableNamespace = null,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsDataTable(conn, procedureName, tableName: tableName, tableNamespace: tableNamespace, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="tableName">An optional name to assign to the <c>DataTable</c>, default is <c>procedureName</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="UtilityException"></exception>
            public static DataTable AsDataTable
            (
                OleDbConnection conn,
                string procedureName,
                string tableName = null,
                string tableNamespace = null,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                DataTable result;
                if (tableNamespace != null)
                {
                    result = new DataTable(tableName ?? procedureName, tableNamespace);
                }
                else
                {
                    result = new DataTable(tableName ?? procedureName);
                }
                using (var command = OleDbUtil.BuildProcedureCommand(conn, procedureName, parameters, transaction, commandTimeout, peekCommand))
                {
                    if (dbCapture != null)
                    {
                        dbCapture.OutputParameters = command.Parameters
                            .Cast<OleDbParameter>()
                            .Where(p => p.Direction == ParameterDirection.Output)
                            .ToArray();
                    }
                    using (var adapter = new OleDbDataAdapter(command))
                    {
                        journal.WriteEntry("adapter.Fill()");
                        adapter.Fill(result);
                    }
                }

                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    throw new ValidationException("Zap does not work on data tables");
                if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    DbUtil.TrimDataTable(result);

                // finalize
                journal.Level--;
                return result;
            }
        }

        /// <summary>
        /// Factory methods for executing functions
        /// </summary>
        public static class TableFunction
        {
            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsCollection<T>(conn, provider, functionName, parameters: parameters, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsCollectionAsync<T>(conn, provider, functionName, parameters: parameters, rowParser: rowParser, sorter: sorter, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                var result = BuildList
                (
                    conn,
                    statement,
                    null,
                    transaction,
                    null,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection. 
            /// The data can be parsed deliberately (via explicit user-supplied parser) or, by default, automatically (mapped from DB column names) into a collection.
            /// </summary>
            /// <typeparam name="T">The type of items to return in the collection.</typeparam>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="transaction">A transaction can encapsulate multiple DML commands including the ability to roll them all back.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="sorter">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbTransaction transaction = null,
                RowParser<T> rowParser = null,
                ListSorter<T> sorter = null,
                AutoTruncate autoTrunc = default,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                var result = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    transaction,
                    null,
                    rowParser,
                    sorter,
                    autoTrunc,
                    commandTimeout,
                    peekCommand,
                    journal
                );

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsObjects(conn, provider, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsObjectsAsync(conn, provider, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = AsDataReader(conn, provider, functionName, parameters: parameters, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
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
                using (var reader = await AsDataReaderAsync(conn, provider, functionName, parameters: parameters, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsScalar(conn, provider, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = await AsScalarAsync(conn, provider, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    result = DbUtil.ProcessScalarResult(result, autoTrunc: autoTrunc);
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = AsDataReader(conn, provider, functionName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var conn = OleDbUtil.LaunchConnection(connectionInfo, peekConnection: peekConnection, journal: journal);
                var result = await AsDataReaderAsync(conn, provider, functionName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand);  // not including parameters here due to already embedded in the SQL statement
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand);  // not including parameters here due to already embedded in the SQL statement
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OleDbParameter>()
                        .Where(p => p.Direction == ParameterDirection.Output)
                        .ToArray();
                }

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekConnection">Allows access to the underlying DB connection prior to command execution</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                DbProvider provider,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
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
                    var result = AsDataTable(conn, provider, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Returns the data as a <see cref="DataTable"/>.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="tableName">An optional name to assign to the <c>DataTable</c>, default is <c>functionName</c></param>
            /// <param name="tableNamespace">An optional namespace to assign to the <c>DataTable</c></param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="peekCommand">Allows access to the underlying DB command prior to execution</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="UtilityException"></exception>
            public static DataTable AsDataTable
            (
                OleDbConnection conn,
                DbProvider provider,
                string functionName,
                string tableName = null,
                string tableNamespace = null,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
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
                var statement = DbUtil.BuildFunctionStatement(provider, functionName, parameters: parameters, journal: journal);
                DataTable result;
                if (tableNamespace != null)
                {
                    result = new DataTable(tableName ?? functionName, tableNamespace);
                }
                else
                {
                    result = new DataTable(tableName ?? functionName);
                }
                using (var command = OleDbUtil.BuildTextCommand(conn, statement, null, null, commandTimeout, peekCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    using (var adapter = new OleDbDataAdapter(command))
                    {
                        journal.WriteEntry("adapter.Fill()");
                        adapter.Fill(result);
                    }
                }

                if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                    throw new ValidationException("Zap does not work on data tables");
                if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                    DbUtil.TrimDataTable(result);

                // finalize
                journal.Level--;
                return result;
            }
        }

        internal static List<T> BuildList<T>
        (
            OleDbConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            OleDbTransaction transaction,
            DbCapture dbCapture,
            RowParser<T> rowParser,
            ListSorter<T> sorter,
            AutoTruncate autoTrunc,
            int? commandTimeout,
            Action<OleDbCommand> peekCommand,
            TraceJournal journal
        )
        {
            journal.WriteEntry("BuildList()");
            journal.Level++;

            var list = new List<T>();
            var fromStoredProc = parameters != null;
            rowParser = rowParser ?? RowParser.BuildAutoParser<T>();

            if (rowParser.IsObjectParser)
            {
                journal.WriteEntry("rowParser.IsObjectParser" + (fromStoredProc ? " fromStoredProc" : ""));
                if (fromStoredProc)
                {
                    using (var command = OleDbUtil.BuildProcedureCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))
                    {
                        var objectArrays = DbUtil.ReadAsObjects(command, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                    {
                        var objectArrays = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
            }
            else if (rowParser.IsReaderParser)
            {
                journal.WriteEntry("rowParser.IsReaderParser" + (fromStoredProc ? " fromStoredProc" : ""));
                if (fromStoredProc)
                {
                    using (var command = OleDbUtil.BuildProcedureCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))
                    {
                        list.AddRange(DbUtil.ParseRows(command, rowParser.ReaderParser, dbCapture: dbCapture, journal: journal));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                    {
                        while (reader.Read())
                        {
                            list.Add(rowParser.Parse(reader));
                        }
                    }
                }
            }
            else
            {
                throw new ThisShouldNeverHappenException("No row parser.");
            }

            if (sorter != null)
            {
                list = sorter.Sort(list, journal: journal);
            }

            journal.Level--;
            return list;
        }

        internal static async Task<List<T>> BuildListAsync<T>
        (
            OleDbConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            OleDbTransaction transaction,
            DbCapture dbCapture,
            RowParser<T> rowParser,
            ListSorter<T> sorter,
            AutoTruncate autoTrunc,
            int? commandTimeout,
            Action<OleDbCommand> peekCommand,
            TraceJournal journal
        )
        {
            journal.WriteEntry("BuildListAsync()");
            journal.Level++;

            var list = new List<T>();
            var fromStoredProc = parameters != null;
            rowParser = rowParser ?? RowParser.BuildAutoParser<T>();

            if (rowParser.IsObjectParser)
            {
                journal.WriteEntry("rowParser.IsObjectParser" + (fromStoredProc ? " fromStoredProc" : ""));
                if (fromStoredProc)
                {
                    using (var command = OleDbUtil.BuildProcedureCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))
                    {
                        var objectArrays = await DbUtil.ReadAsObjectsAsync(command, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                    {
                        var objectArrays = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
            }
            else if (rowParser.IsReaderParser)
            {
                journal.WriteEntry("rowParser.IsReaderParser" + (fromStoredProc ? " fromStoredProc" : ""));
                if (fromStoredProc)
                {
                    using (var command = OleDbUtil.BuildProcedureCommand(conn, statement, parameters, transaction, commandTimeout, peekCommand))
                    {
                        list.AddRange(await DbUtil.ParseRowsAsync(command, rowParser.ReaderParser, dbCapture: dbCapture, journal: journal));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, peekCommand: peekCommand, journal: journal))
                    {
                        while (reader.Read())
                        {
                            list.Add(rowParser.Parse(reader));
                        }
                    }
                }
            }
            else
            {
                throw new ThisShouldNeverHappenException("No row parser.");
            }

            if (sorter != null)
            {
                list = sorter.Sort(list, journal: journal);
            }

            journal.Level--;
            return list;
        }
    }
}
