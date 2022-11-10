using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextClean;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Query
    {
        public static class SQL
        {
            public static IEnumerable<T> AsCollection<T>
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsCollection(conn, statement, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsCollectionAsync(conn, statement, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<T> AsCollection<T>
            (
                OracleConnection conn,
                string statement,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = BuildList
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser, 
                    readerParser,
                    autoSort, 
                    autoTrunc,
                    timeout, 
                    modifyCommand
                );
                return list;
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OracleConnection conn,
                string statement,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser,
                    readerParser,
                    autoSort,
                    autoTrunc,
                    timeout,
                    modifyCommand
                );
                return list;
            }

            public static object AsScalar
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsScalar(conn, statement, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<object> AsScalarAsync
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsScalarAsync(conn, statement, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static object AsScalar
            (
                OracleConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            var obj = cmd.ExecuteScalar();
                            if (ObjectUtil.IsNull(obj)) return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<object> AsScalarAsync
            (
                OracleConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            var obj = await cmd.ExecuteScalarAsync();
                            if (ObjectUtil.IsNull(obj)) return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static OracleDataReader AsDataReader
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataReader(conn, statement, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsDataReaderAsync(conn, statement, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static OracleDataReader AsDataReader
            (
                OracleConnection conn,
                string statement,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? cmd.ExecuteReader(CommandBehavior.Default)
                            : cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        return reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                OracleConnection conn,
                string statement,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? await cmd.ExecuteReaderAsync(CommandBehavior.Default)
                            : await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                        return (OracleDataReader)reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static DataTable AsDataTable
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataTable(conn, statement, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static DataTable AsDataTable
            (
                OracleConnection conn,
                string statement,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var dataTable = new DataTable();
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            using (var adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                DbUtil.TrimDataTable(dataTable);
                                break;
                            case AutoTruncate.Zap:
                            case AutoTruncate.ZapEmptyStringsOnly:
                                throw new UtilityException("Zap does not work on data tables");
                        }
                        return dataTable;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsObjects(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsObjectsAsync(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                OracleConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OracleConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<object[]>();
                using (var reader = await AsDataReaderAsync(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsStrings(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                string statement,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsStringsAsync(conn, statement, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                OracleConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                OracleConnection conn,
                string statement,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<string[]>();
                using (var reader = await AsDataReaderAsync(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }
        }

        public static class TableOrView
        {
            public static IEnumerable<T> AsCollection<T>
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsCollection(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, timeout: timeout, autoTrunc: autoTrunc, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsCollectionAsync(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static IEnumerable<T> AsCollection<T>
            (
                OracleConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy, peekStatement);
                var list = BuildList
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser,
                    readerParser,
                    autoSort,
                    autoTrunc,
                    timeout,
                    modifyCommand
                );
                return list;
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OracleConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy, peekStatement);
                var list = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser,
                    readerParser,
                    autoSort,
                    autoTrunc,
                    timeout,
                    modifyCommand
                );
                return list;
            }

            static string BuildStatement
            (
                string tableOrViewName,
                IEnumerable<string> columns,
                Filter where,
                IEnumerable<string> groupBy,
                IEnumerable<string> orderBy,
                Action<string> peekStatement
            )
            {
                var statement = @"
                    SELECT " + (columns != null ? string.Join(", ", columns) : "*") + @"
                    FROM " + tableOrViewName;
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
                peekStatement?.Invoke(statement);

                return statement;
            }

            public static object AsScalar
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                string column = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsScalar(conn, tableOrViewName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<object> AsScalarAsync
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                string column = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsScalarAsync(conn, tableOrViewName, column: column, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, modifyCommand: modifyCommand);
                }
            }

            public static object AsScalar
            (
                OracleConnection conn,
                string tableOrViewName,
                string column = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var statement = BuildStatement(tableOrViewName, column != null ? new[] { column } : null, where, groupBy, orderBy, peekStatement);
                using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var obj = cmd.ExecuteScalar();
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                return stringValue.Trim();
                            case AutoTruncate.Zap:
                                return Zap.String(stringValue);
                            case AutoTruncate.ZapEmptyStringsOnly:
                                if (stringValue.Length == 0)
                                    return null;
                                return stringValue;
                        }
                    }
                    return obj;
                }
            }

            public static async Task<object> AsScalarAsync
            (
                OracleConnection conn,
                string tableOrViewName,
                string column = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var statement = BuildStatement(tableOrViewName, column != null ? new[] { column } : null, where, groupBy, orderBy, peekStatement);
                using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var obj = await cmd.ExecuteScalarAsync();
                    if (ObjectUtil.IsNull(obj))
                        return null;
                    if (obj is string stringValue)
                    {
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                return stringValue.Trim();
                            case AutoTruncate.Zap:
                                return Zap.String(stringValue);
                            case AutoTruncate.ZapEmptyStringsOnly:
                                if (stringValue.Length == 0)
                                    return null;
                                return stringValue;
                        }
                    }
                    return obj;
                }
            }

            public static OracleDataReader AsDataReader
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo);
                return AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo);
                return await AsDataReaderAsync(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
            }

            public static OracleDataReader AsDataReader
            (
                OracleConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy, peekStatement);
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? cmd.ExecuteReader(CommandBehavior.Default)
                            : cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        return reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                OracleConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy, peekStatement);
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? await cmd.ExecuteReaderAsync(CommandBehavior.Default)
                            : await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                        return (OracleDataReader)reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static DataTable AsDataTable
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataTable(conn, tableOrViewName, columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static DataTable AsDataTable
            (
                OracleConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var dataTable = new DataTable(tableOrViewName);
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy, peekStatement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 3)
                {
                    bool reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            using (var adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                DbUtil.TrimDataTable(dataTable);
                                break;
                            case AutoTruncate.Zap:
                            case AutoTruncate.ZapEmptyStringsOnly:
                                throw new ValidationException("Zap does not work on data tables");
                        }
                        return dataTable;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsObjects(conn, tableOrViewName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsObjectsAsync(conn, tableOrViewName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                OracleConnection conn,
                string tableOrViewName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OracleConnection conn,
                string tableOrViewName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<object[]>();
                using (var reader = await AsDataReaderAsync(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsStrings(conn, tableOrViewName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                string tableOrViewName,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsStringsAsync(conn, tableOrViewName, dbCapture: dbCapture, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                OracleConnection conn,
                string tableOrViewName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                OracleConnection conn,
                string tableOrViewName,
                DbCapture dbCapture = null,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<string[]>();
                using (var reader = await AsDataReaderAsync(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }
        }

        public static class Procedure
        {
            public static IEnumerable<T> AsCollection<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsCollection<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsCollectionAsync<T>(conn, procedureName, dbCapture: dbCapture, parameters: parameters, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<T> AsCollection<T>
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        var list = BuildList
                        (
                            conn,
                            procedureName,
                            parameters ?? Enumerable.Empty<DbParameter>(),
                            dbCapture,
                            objectParser,
                            readerParser,
                            autoSort,
                            autoTrunc,
                            timeout,
                            modifyCommand
                        );
                        return list;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        var list = await BuildListAsync
                        (
                            conn,
                            procedureName,
                            parameters ?? Enumerable.Empty<DbParameter>(),
                            dbCapture,
                            objectParser,
                            readerParser,
                            autoSort,
                            autoTrunc,
                            timeout,
                            modifyCommand
                        );
                        return list;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static OracleDataReader AsDataReader
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static OracleDataReader AsDataReader
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? cmd.ExecuteReader(CommandBehavior.Default)
                            : cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (dbCapture != null)
                        {
                            dbCapture.OutputParameters = cmd.Parameters
                                .Cast<OracleParameter>()
                                .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                .Select(p => new Parameter(p.ParameterName, p.Value))
                                .ToArray();
                        }
                        return reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand);
                        var reader = keepOpen
                            ? await cmd.ExecuteReaderAsync(CommandBehavior.Default)
                            : await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                        if (dbCapture != null)
                        {
                            dbCapture.OutputParameters = cmd.Parameters
                                .Cast<OracleParameter>()
                                .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                .Select(p => new Parameter(p.ParameterName, p.Value))
                                .ToArray();
                        }
                        return (OracleDataReader)reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static DataTable AsDataTable
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataTable(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static DataTable AsDataTable
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        var dataTable = new DataTable(procedureName);
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            if (dbCapture != null)
                            {
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            using (var adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                DbUtil.TrimDataTable(dataTable);
                                break;
                            case AutoTruncate.Zap:
                            case AutoTruncate.ZapEmptyStringsOnly:
                                throw new UtilityException("Zap does not work on data tables");
                        }
                        return dataTable;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsObjects(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsObjectsAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<object[]>();
                using (var reader = await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsStrings(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsStringsAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<string[]>> AsStringsAsync
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                var list = new List<string[]>();
                using (var reader = await AsDataReaderAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }

            public static object AsScalar
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsScalar(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static async Task<object> AsScalarAsync
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsScalarAsync(conn, procedureName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand);
                }
            }

            public static object AsScalar
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            var obj = cmd.ExecuteScalar();
                            if (dbCapture != null)
                            {
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            if (ObjectUtil.IsNull(obj))
                                return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<object> AsScalarAsync
            (
                OracleConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null
            )
            {
                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    bool reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                        {
                            var obj = await cmd.ExecuteScalarAsync();
                            if (dbCapture != null)
                            {
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            if (ObjectUtil.IsNull(obj))
                                return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }
        }

        public static class TableFunction
        {
            public static IEnumerable<T> AsCollection<T>
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsCollection<T>(conn, functionName, parameters: parameters, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsCollectionAsync<T>(conn, functionName, parameters: parameters, objectParser: objectParser, readerParser: readerParser, autoSort: autoSort, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static IEnumerable<T> AsCollection<T>
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "())";
                }
                else
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + "))";
                }
                peekStatement?.Invoke(statement);

                var list = BuildList
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser,
                    readerParser,
                    autoSort,
                    autoTrunc,
                    timeout,
                    modifyCommand
                );
                return list;
            }

            public static async Task<IEnumerable<T>> AsCollectionAsync<T>
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                Func<object[], T> objectParser = null,
                Func<IDataReader, T> readerParser = null,
                AutoSort<T> autoSort = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "())";
                }
                else
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + "))";
                }
                peekStatement?.Invoke(statement);

                var list = await BuildListAsync
                (
                    conn,
                    statement,
                    null,
                    null,
                    objectParser,
                    readerParser,
                    autoSort,
                    autoTrunc,
                    timeout,
                    modifyCommand
                );
                return list;
            }

            public static object AsScalar
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsScalar(conn, functionName, parameters: parameters, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<object> AsScalarAsync
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsScalarAsync(conn, functionName, parameters: parameters, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static object AsScalar
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT " + functionName + "() FROM DUAL";
                }
                else
                {
                    statement = "SELECT " + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + ") FROM DUAL";
                }
                peekStatement?.Invoke(statement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))  // not including parameters here due to already embedded in the SQL statement
                        {
                            var obj = cmd.ExecuteScalar();
                            if (ObjectUtil.IsNull(obj))
                                return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<object> AsScalarAsync
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT " + functionName + "() FROM DUAL";
                }
                else
                {
                    statement = "SELECT " + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + ") FROM DUAL";
                }
                peekStatement?.Invoke(statement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))  // not including parameters here due to already embedded in the SQL statement
                        {
                            var obj = await cmd.ExecuteScalarAsync();
                            if (ObjectUtil.IsNull(obj))
                                return null;
                            if (obj is string stringValue)
                            {
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        return stringValue.Trim();
                                    case AutoTruncate.Zap:
                                        return Zap.String(stringValue);
                                    case AutoTruncate.ZapEmptyStringsOnly:
                                        if (stringValue.Length == 0)
                                            return null;
                                        return stringValue;
                                }
                            }
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static OracleDataReader AsDataReader
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataReader(conn, functionName, parameters: parameters, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsDataReaderAsync(conn, functionName, parameters: parameters, keepOpen: keepOpen, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static OracleDataReader AsDataReader
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "())";
                }
                else
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + "))";
                }
                peekStatement?.Invoke(statement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);  // not including parameters here due to already embedded in the SQL statement
                        var reader = keepOpen
                            ? cmd.ExecuteReader(CommandBehavior.Default)
                            : cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        return reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static async Task<OracleDataReader> AsDataReaderAsync
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                bool keepOpen = false,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "())";
                }
                else
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + "))";
                }
                peekStatement?.Invoke(statement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand);  // not including parameters here due to already embedded in the SQL statement
                        var reader = keepOpen
                            ? await cmd.ExecuteReaderAsync(CommandBehavior.Default)
                            : await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                        return (OracleDataReader)reader;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (!keepOpen)
                        {
                            try { conn.Close(); } finally { }
                        }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static DataTable AsDataTable
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsDataTable(conn, functionName, parameters: parameters, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static DataTable AsDataTable
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "())";
                }
                else
                {
                    statement = "SELECT * FROM TABLE(" + functionName + "(" + string.Join(", ", parameters.Select(p => DbUtil.Sqlize(p.Value, DbPlatform.Oracle))) + "))";
                }
                peekStatement?.Invoke(statement);

                int iteration = 0;
                OracleException oraEx = null;
                while (iteration++ < 2)
                {
                    var reattempting = false;
                    try
                    {
                        var dataTable = new DataTable(functionName);
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout, modifyCommand: modifyCommand))  // not including parameters here due to already embedded in the SQL statement
                        {
                            using (var adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                DbUtil.TrimDataTable(dataTable);
                                break;
                            case AutoTruncate.Zap:
                            case AutoTruncate.ZapEmptyStringsOnly:
                                throw new UtilityException("Zap does not work on data tables");
                        }
                        return dataTable;
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Message.Contains("ORA-02396") || ex.Message.Contains("please connect again"))
                        {
                            oraEx = ex;
                            reattempting = true;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        try { conn.Close(); } finally { }
                        if (reattempting)
                        {
                            Thread.Sleep(OracleDbSettings.ReattemptInterval);
                            conn = OracleDbUtil.LaunchConnection(conn);
                        }
                    }
                }
                throw oraEx ?? new UtilityException("Exceeded max attempts") as Exception;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return AsObjects(conn, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                OracleDbConnectionInfo connectionInfo = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<OracleDbConnectionInfo> resultantConnectionInfo = null,
                Action<string> peekStatement = null
            )
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo, resultantConnectionInfo: resultantConnectionInfo))
                {
                    return await AsObjectsAsync(conn, functionName, parameters: parameters, dbCapture: dbCapture, autoTrunc: autoTrunc, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, functionName, parameters: parameters, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (reader.Read())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static async Task<IEnumerable<object[]>> AsObjectsAsync
            (
                OracleConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                DbCapture dbCapture = null,
                AutoTruncate autoTrunc = default,
                int? timeout = null,
                Action<OracleCommand> modifyCommand = null,
                Action<string> peekStatement = null
            )
            {
                var list = new List<object[]>();
                using (var reader = await AsDataReaderAsync(conn, functionName, parameters: parameters, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand, peekStatement: peekStatement))
                {
                    var cols = reader.GetDataColumns();
                    if (dbCapture != null)
                        dbCapture.DataColumns = cols;
                    while (await reader.ReadAsync())
                    {
                        var objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                        list.Add(objects);
                    }
                }
                return list;
            }
        }

        internal static List<T> BuildList<T>
        (
            OracleConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            DbCapture dbCapture,
            Func<object[], T> objectParser,
            Func<IDataReader, T> readerParser,
            AutoSort<T> autoSort,
            AutoTruncate autoTrunc,
            int? timeout,
            Action<OracleCommand> modifyCommand
        )
        {
            var list = new List<T>();
            var fromStoredProc = parameters != null;

            if (objectParser != null)
            {
                if (readerParser != null) 
                    throw new UtilityException("This method accepts a maximum of one parser");

                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var cols = reader.GetDataColumns();
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = cols;
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            object[] objects;
                            while (reader.Read())
                            {
                                objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                                list.Add(objectParser.Invoke(objects));
                            }
                        }
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        var cols = reader.GetDataColumns();
                        object[] objects;
                        while (reader.Read())
                        {
                            objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                            list.Add(objectParser.Invoke(objects));
                        }
                    }
                }
            }
            else if (readerParser != null)
            {
                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = reader.GetDataColumns();
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            while (reader.Read())
                            {
                                list.Add(readerParser.Invoke(reader));
                            }
                        }
                    }
                }
                else
                {
                    using (var reader = SQL.AsDataReader(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        while (reader.Read())
                        {
                            list.Add(readerParser.Invoke(reader));
                        }
                    }
                }
            }
            else
            {
                if (!typeof(T).IsClass || !typeof(T).GetConstructors().Any(c => !c.GetParameters().Any()))
                {
                    throw new UtilityException("Cannot auto-generate instances of " + typeof(T).FullName + ".  Use only classes with a parameterless constructor.");
                }
                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = reader.GetDataColumns();
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            BuildAndAddInstances
                            (
                                list,
                                reader,
                                autoTrunc
                            );
                        }
                    }
                }
                else
                {
                    var _dbCapture = dbCapture ?? new DbCapture();
                    var objectArrays = SQL.AsObjects(conn, statement, dbCapture: _dbCapture, timeout: timeout, autoTrunc: autoTrunc, modifyCommand: modifyCommand);
                    BuildAndAddInstances
                    (
                        list,
                        objectArrays,
                        _dbCapture.DataColumns
                    );
                }
            }
            if (autoSort != null)
            {
                list = AutoSortList
                (
                    list,
                    autoSort
                );
            }
            return list;
        }

        internal static async Task<List<T>> BuildListAsync<T>
        (
            OracleConnection conn,
            string statement,
            IEnumerable<DbParameter> parameters,
            DbCapture dbCapture,
            Func<object[], T> objectParser,
            Func<IDataReader, T> readerParser,
            AutoSort<T> autoSort,
            AutoTruncate autoTrunc,
            int? timeout,
            Action<OracleCommand> modifyCommand
        )
        {
            var list = new List<T>();
            var fromStoredProc = parameters != null;

            if (objectParser != null)
            {
                if (readerParser != null)
                    throw new UtilityException("This method accepts a maximum of one parser");

                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var cols = reader.GetDataColumns();
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = cols;
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            object[] objects;
                            while (await reader.ReadAsync())
                            {
                                objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                                list.Add(objectParser.Invoke(objects));
                            }
                        }
                    }
                }
                else
                {
                    using (var reader = await SQL.AsDataReaderAsync(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        var cols = reader.GetDataColumns();
                        object[] objects;
                        while (await reader.ReadAsync())
                        {
                            objects = DbUtil.GetAllValues(reader, cols.Length, autoTrunc: autoTrunc);
                            list.Add(objectParser.Invoke(objects));
                        }
                    }
                }
            }
            else if (readerParser != null)
            {
                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = reader.GetDataColumns();
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            while (await reader.ReadAsync())
                            {
                                list.Add(readerParser.Invoke(reader));
                            }
                        }
                    }
                }
                else
                {
                    using (var reader = await SQL.AsDataReaderAsync(conn, statement, keepOpen: true, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(readerParser.Invoke(reader));
                        }
                    }
                }
            }
            else
            {
                if (!typeof(T).IsClass || !typeof(T).GetConstructors().Any(c => !c.GetParameters().Any()))
                {
                    throw new UtilityException("Cannot auto-generate instances of " + typeof(T).FullName + ".  Use only classes with a parameterless constructor.");
                }
                if (fromStoredProc)
                {
                    using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, statement, parameters: parameters, timeout: timeout, modifyCommand: modifyCommand))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (dbCapture != null)
                            {
                                dbCapture.DataColumns = reader.GetDataColumns();
                                dbCapture.OutputParameters = cmd.Parameters
                                    .Cast<OracleParameter>()
                                    .Where(p => p.Direction == ParameterDirection.Output && p.OracleDbType != OracleDbType.RefCursor)
                                    .Select(p => new Parameter(p.ParameterName, p.Value))
                                    .ToArray();
                            }
                            await BuildAndAddInstancesAsync
                            (
                                list,
                                (OracleDataReader)reader,
                                autoTrunc
                            );
                        }
                    }
                }
                else
                {
                    var _dbCapture = dbCapture ?? new DbCapture();
                    var objectArrays = SQL.AsObjects(conn, statement, dbCapture: _dbCapture, timeout: timeout, autoTrunc: autoTrunc, modifyCommand: modifyCommand);
                    BuildAndAddInstances
                    (
                        list,
                        objectArrays,
                        _dbCapture.DataColumns
                    );
                }
            }
            if (autoSort != null)
            {
                list = AutoSortList
                (
                    list,
                    autoSort
                );
            }
            return list;
        }

        internal static void BuildAndAddInstances<T>
        (
            List<T> list,
            IEnumerable<object[]> objectArrays,
            DataColumn[] dataColumns
        )
        {
            var normalizedColumnNames = dataColumns
                .Select(c => Zap.String(c.ColumnName, preProcess: (n) => TextClean.RemoveWhitespace(n)))
                .ToArray();
            foreach (var objects in objectArrays)
            {
                T e = (T)ObjectUtil.GetInstance(typeof(T));
                for (int i = 0; i < objects.Length; i++)
                {
                    ObjectUtil.SetInstancePropertyValue(e, normalizedColumnNames[i], objects[i], ignoreCase: true);
                }
                list.Add(e);
            }
        }

        internal static void BuildAndAddInstances<T>
        (
            List<T> list,
            OracleDataReader reader,
            AutoTruncate autoTrunc
        )
        {
            var dataColumns = reader.GetDataColumns();
            var normalizedColumnNames = dataColumns
                .Select(c => Zap.String(c.ColumnName, preProcess: (n) => TextClean.RemoveWhitespace(n)))
                .ToArray();
            object[] objects;
            while (reader.Read())
            {
                objects = DbUtil.GetAllValues(reader, dataColumns.Length, autoTrunc: autoTrunc);
                T e = (T)ObjectUtil.GetInstance(typeof(T));
                for (int i = 0; i < objects.Length; i++)
                {
                    ObjectUtil.SetInstancePropertyValue(e, normalizedColumnNames[i], objects[i], ignoreCase: true);
                }
                list.Add(e);
            }
        }

        internal static async Task BuildAndAddInstancesAsync<T>
        (
            List<T> list,
            OracleDataReader reader,
            AutoTruncate autoTrunc
        )
        {
            var dataColumns = reader.GetDataColumns();
            var normalizedColumnNames = dataColumns
                .Select(c => Zap.String(c.ColumnName, preProcess: (n) => TextClean.RemoveWhitespace(n)))
                .ToArray();
            object[] objects;
            while (await reader.ReadAsync())
            {
                objects = DbUtil.GetAllValues(reader, dataColumns.Length, autoTrunc: autoTrunc);
                T e = (T)ObjectUtil.GetInstance(typeof(T));
                for (int i = 0; i < objects.Length; i++)
                {
                    ObjectUtil.SetInstancePropertyValue(e, normalizedColumnNames[i], objects[i], ignoreCase: true);
                }
                list.Add(e);
            }
        }

        internal static List<T> AutoSortList<T>
        (
            List<T> list,
            AutoSort<T> autoSort
        )
        {
            if (autoSort != null)
            {
                if (autoSort.Sorter != null)
                {
                    list = list
                        .OrderBy(autoSort.Sorter)
                        .ToList();
                }
                else if (autoSort.Comparer != null)
                {
                    list.Sort(autoSort.Comparer);
                }
                else if (autoSort.Comparison != null)
                {
                    list.Sort(autoSort.Comparison);
                }
                return list;
            }
            list.Sort();
            return list;
        }
    }
}
