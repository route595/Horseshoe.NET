using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsCollection(conn, statement, dbCapture: dbCapture, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsCollectionAsync(conn, statement, dbCapture: dbCapture, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = BuildList
                (
                    conn,
                    statement,
                    null,
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsObjects(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsObjectsAsync(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = AsDataReader(conn, statement, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = await AsDataReaderAsync(conn, statement, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsScalar(conn, statement, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsScalarAsync(conn, statement, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OdbcConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(result))
                        return null;
                    if (result is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
                    return result;
                }
            }

            /// <summary>
            /// Executes a SQL statement using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="statement">Typically a SQL 'select' statement</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OdbcConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(result))
                        return null;
                    if (result is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = await AsDataReaderAsync(conn, statement, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OdbcConnection conn,
                string statement,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string statement,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsDataTable(conn, statement, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="ValidationException"></exception>
            public static DataTable AsDataTable
            (
                OdbcConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = new DataTable();
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    using (var adapter = new OdbcDataAdapter(command))
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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsCollection(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsCollectionAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OdbcConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OdbcConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsObjects(conn, tableName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsObjectsAsync(conn, tableName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OdbcConnection conn,
                string tableName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = AsDataReader(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OdbcConnection conn,
                string tableName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = await AsDataReaderAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsScalar(conn, tableName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsScalarAsync(conn, tableName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, commandTimeout: commandTimeout, autoTrunc: autoTrunc, alterCommand: alterCommand, journal: journal);

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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OdbcConnection conn,
                string tableName,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = BuildStatement
                (
                    tableName,
                    column != null ? new[] { column } : null,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    var result = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(result))
                        return null;
                    if (result is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OdbcConnection conn,
                string tableName,
                string column = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = BuildStatement
                (
                    tableName,
                    column != null ? new[] { column } : null,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    var result = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(result))
                        return null;
                    if (result is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = AsDataReader(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = await AsDataReaderAsync(conn, tableName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OdbcConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OdbcConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="tableName">Typically a table or view to query.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string tableName,
                OdbcConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsDataTable(conn, tableName, columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="columns">The columns in the table or view to return in the result.</param>
            /// <param name="where">A filter which renders to a SQL 'where' clause.</param>
            /// <param name="groupBy">A column name or names to render to a SQL 'group by' clause.</param>
            /// <param name="orderBy">A column name or names to render to a SQL 'order by' clause for server-side row ordering.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="ValidationException"></exception>
            public static DataTable AsDataTable
            (
                OdbcConnection conn,
                string tableName,
                IEnumerable<string> columns = null,
                IFilter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = new DataTable(tableName);
                var statement = BuildStatement
                (
                    tableName,
                    columns,
                    where,
                    groupBy,
                    orderBy,
                    journal
                );

                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))
                {
                    using (var adapter = new OdbcDataAdapter(command))
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
                // trace journaling
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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsCollection<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsCollectionAsync<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = BuildList
                (
                    conn,
                    procedureName,
                    parameters ?? Enumerable.Empty<DbParameter>(),
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = await BuildListAsync
                (
                    conn,
                    procedureName,
                    parameters ?? Enumerable.Empty<DbParameter>(),
                    dbCapture,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsObjects(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsObjectsAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = DbUtil.ReadAsObjects(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
                {
                    var result = await DbUtil.ReadAsObjectsAsync(reader, dbCapture: dbCapture, autoTrunc: autoTrunc);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsScalar(conn, procedureName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsScalarAsync(conn, procedureName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var command = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, commandTimeout, alterCommand))
                {
                    var obj = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
                    return obj;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied stored procedure using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="procedureName">The name of the stored procedure being queried.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var command = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, commandTimeout, alterCommand))
                {
                    var obj = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
                    return obj;
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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var command = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, commandTimeout, alterCommand);
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var command = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, commandTimeout, alterCommand);
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsDataTable(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="UtilityException"></exception>
            public static DataTable AsDataTable
            (
                OdbcConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var result = new DataTable(procedureName);
                using (var command = OdbcUtil.BuildProcedureCommand(conn, procedureName, parameters, commandTimeout, alterCommand))
                {
                    if (dbCapture != null)
                    {
                        dbCapture.OutputParameters = command.Parameters
                            .Cast<OdbcParameter>()
                            .Where(p => p.Direction == ParameterDirection.Output)
                            .ToArray();
                    }
                    using (var adapter = new OdbcDataAdapter(command))
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsCollection<T>(conn, platform, functionName, parameters: parameters, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsCollectionAsync<T>(conn, platform, functionName, parameters: parameters, rowParser: rowParser, autoSort: autoSort, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static IEnumerable<T> AsCollection<T>
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                var result = BuildList
                (
                    conn,
                    statement,
                    null,
                    null,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="rowParser">Builds an instance of <c>T</c> from row data.</param>
            /// <param name="autoSort">A mechanism for sorting instances of <c>T</c> before returning them to the caller.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as a colletion of <c>T</c>.</returns>
            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                RowParser<T> rowParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                var result = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    null,
                    rowParser,
                    autoSort,
                    autoTrunc,
                    commandTimeout,
                    alterCommand,
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsObjects(conn, platform, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Data is presented as plain <c>object[]</c>s.
            /// </summary>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsObjectsAsync(conn, platform, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static IEnumerable<object[]> AsObjects
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = AsDataReader(conn, platform, functionName, parameters: parameters, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The data as <c>object[]</c>s.</returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var reader = await AsDataReaderAsync(conn, platform, functionName, parameters: parameters, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsScalar(conn, platform, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                    // finalize
                    journal.Level--;
                    return result;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = await AsScalarAsync(conn, platform, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static object AsScalar
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    var obj = command.ExecuteScalar();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
                    return obj;
                }
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection.
            /// Returns the selected datum or the first field of the first row of the result set.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The first field of the first row of the result set.</returns>
            public static async Task<object> AsScalarAsync
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    var obj = await command.ExecuteScalarAsync();
                    journal.Level--;  // finalize
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        if ((autoTrunc & AutoTruncate.Zap) == AutoTruncate.Zap)
                        {
                            if ((autoTrunc & AutoTruncate.EmptyStringsOnly) == AutoTruncate.EmptyStringsOnly)
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    if (stringValue.Length == 0)
                                        stringValue = null;
                                }
                                else
                                    stringValue = stringValue.Trim();
                            }
                            else
                            {
                                stringValue = stringValue.Trim();
                                if (stringValue.Length == 0)
                                    stringValue = null;
                            }
                        }
                        else if ((autoTrunc & AutoTruncate.Trim) == AutoTruncate.Trim)
                            stringValue = stringValue.Trim();
                        return stringValue;
                    }
                    return obj;
                }
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = AsDataReader(conn, platform, functionName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Creates a database connection and executes a query on the user-supplied function. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal);
                var result = await AsDataReaderAsync(conn, platform, functionName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

                // finalize
                journal.Level--;
                return result;
            }

            /// <summary>
            /// Executes a query on the user-supplied function using an existing open connection. 
            /// Returns the raw data reader.
            /// </summary>
            /// <param name="conn">An open DB connection.</param>
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static DbDataReader AsDataReader
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);  // not including parameters here due to already embedded in the SQL statement
                var result = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="dbCapture">A <c>DbCapture</c> instance stores certain metadata only available during live query execution.</param>
            /// <param name="keepOpen">Whether to keep a live connection open after exposing the reader to the caller.</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>The raw data reader</returns>
            public static async Task<DbDataReader> AsDataReaderAsync
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand);  // not including parameters here due to already embedded in the SQL statement
                var result = keepOpen
                    ? await command.ExecuteReaderAsync(CommandBehavior.Default)
                    : await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dbCapture != null)
                {
                    dbCapture.DataColumns = result.GetDataColumns();
                    dbCapture.OutputParameters = command.Parameters
                        .Cast<OdbcParameter>()
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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="connectionInfo">Connection information e.g. a connection string or the info needed to build one.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            public static DataTable AsDataTable
            (
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OdbcConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                CryptoOptions cryptoOptions = null,
                Action<OdbcCommand> alterCommand = null,
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
                using (var conn = OdbcUtil.LaunchConnection(connectionInfo, cryptoOptions: cryptoOptions, journal: journal))
                {
                    var result = AsDataTable(conn, platform, functionName, parameters: parameters, autoTrunc: autoTrunc, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal);

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
            /// <param name="platform">A DB platform lends hints about how to render SQL expressions or entire SQL statements.</param>
            /// <param name="functionName">The name of the function being called.</param>
            /// <param name="parameters">An optional collection of <c>DbParamerter</c>s to inject into the statement or pass separately into the call.</param>
            /// <param name="autoTrunc">A mechanism for handling raw string data (e.g. 'trim' or 'zap' which nullifies empty strings).</param>
            /// <param name="commandTimeout">The wait time before terminating an attempt to execute a command and generating an error.</param>
            /// <param name="alterCommand">Allows access to the underlying DB command for final inspection or alteration before executing.</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>A <see cref="DataTable"/></returns>
            /// <exception cref="UtilityException"></exception>
            public static DataTable AsDataTable
            (
                OdbcConnection conn,
                DbPlatform platform,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? commandTimeout = null,
                Action<OdbcCommand> alterCommand = null,
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
                var statement = DbUtil.BuildFunctionStatement(platform, functionName, parameters: parameters, journal: journal);
                var result = new DataTable(functionName);
                using (var command = OdbcUtil.BuildTextCommand(conn, statement, null, commandTimeout, alterCommand))  // not including parameters here due to already embedded in the SQL statement
                {
                    using (var adapter = new OdbcDataAdapter(command))
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
            OdbcConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            DbCapture dbCapture,
            RowParser<T> rowParser,
            AutoSort<T> autoSort,
            AutoTruncate autoTrunc,
            int? commandTimeout,
            Action<OdbcCommand> alterCommand,
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
                    using (var command = OdbcUtil.BuildProcedureCommand(conn, statement, parameters, commandTimeout, alterCommand))
                    {
                        var objectArrays = DbUtil.ReadAsObjects(command, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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
                    using (var command = OdbcUtil.BuildProcedureCommand(conn, statement, parameters, commandTimeout, alterCommand))
                    {
                        list.AddRange(DbUtil.ParseRows(command, rowParser.ReaderParser, dbCapture: dbCapture, journal: journal));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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

            if (autoSort != null)
            {
                list = AutoSortList
                (
                    list,
                    autoSort,
                    journal
                );
            }

            journal.Level--;
            return list;
        }

        internal static async Task<List<T>> BuildListAsync<T>
        (
            OdbcConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            DbCapture dbCapture,
            RowParser<T> rowParser,
            AutoSort<T> autoSort,
            AutoTruncate autoTrunc,
            int? commandTimeout,
            Action<OdbcCommand> alterCommand,
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
                    using (var command = OdbcUtil.BuildProcedureCommand(conn, statement, parameters, commandTimeout, alterCommand))
                    {
                        var objectArrays = await DbUtil.ReadAsObjectsAsync(command, dbCapture: dbCapture, autoTrunc: autoTrunc, journal: journal);
                        list.AddRange(objectArrays.Select(objs => rowParser.Parse(objs)));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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
                    using (var command = OdbcUtil.BuildProcedureCommand(conn, statement, parameters, commandTimeout, alterCommand))
                    {
                        list.AddRange(await DbUtil.ParseRowsAsync(command, rowParser.ReaderParser, dbCapture: dbCapture, journal: journal));
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, dbCapture: dbCapture, keepOpen: true, commandTimeout: commandTimeout, alterCommand: alterCommand, journal: journal))
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

            if (autoSort != null)
            {
                list = AutoSortList
                (
                    list,
                    autoSort,
                    journal
                );
            }

            journal.Level--;
            return list;
        }

        internal static List<T> AutoSortList<T>
        (
            List<T> list,
            AutoSort<T> autoSort,
            TraceJournal journal
        )
        {
            journal.WriteEntry("AutoSortList()");
            journal.Level++;

            if (autoSort.Sorter != null)
            {
                journal.WriteEntry("autoSort.Sorter");
                list = list
                    .OrderBy(autoSort.Sorter)
                    .ToList();
            }
            else if (autoSort.Comparer != null)
            {
                journal.WriteEntry("autoSort.Comparer");
                list.Sort(autoSort.Comparer);
            }
            else if (autoSort.Comparison != null)
            {
                journal.WriteEntry("autoSort.Comparison");
                list.Sort(autoSort.Comparison);
            }
            else
            {
                journal.WriteEntry("Sort()");
                list.Sort();
            }

            journal.Level--;
            return list;
        }
    }
}
