using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Compare;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb.Meta
{
    public static class MetaSearch
    {
        public static class Databases
        {
            public static IEnumerable<Db> List(Predicate<Db> filter = null, int? commandTimeout = null)
            {
                var server = SqlDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server, Credentials = SqlDbSettings.DefaultCredentials }))
                {
                    return List(conn, server, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<Db> List(SqlConnection conn, Predicate<Db> filter = null, int? commandTimeout = null)
            {
                var server = new DbServer(conn.DataSource);
                return List(conn, server, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<Db> List(SqlDbConnectionInfo connectionInfo, Predicate<Db> filter = null, int? commandTimeout = null)
            {
                var server = connectionInfo.DataSource ?? throw new UtilityException("DataSource cannot be null");
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: connectionInfo))
                {
                    return List(conn, server, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<Db> List(DbServer server, Credential? credentials = null, Predicate<Db> filter = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("DataSource cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, filter: filter, commandTimeout: commandTimeout);
                }
            }

            internal static IEnumerable<Db> List(out SqlConnection conn, DbServer server, Credential? credentials = null, Predicate<Db> filter = null, int? commandTimeout = null)
            {
                conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("DataSource cannot be null"), Credentials = credentials });
                return List(conn, server, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<Db> List(SqlConnection conn, DbServer server, Predicate<Db> filter = null, int? commandTimeout = null)
            {
                var statement = @"
                    SELECT [name] 
                    FROM [master].[sys].[databases] 
                    WHERE [name] NOT IN ('master', 'msdb', 'tempdb', 'model')
                ";
                var databases = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.From((reader) => new Db(reader["name"] as string) { Parent = server }),
                    sorter: new ListSorter<Db>(db => db.Name),
                    commandTimeout: commandTimeout
                );
                if (filter != null)
                {
                    return databases
                        .Where(db => filter.Invoke(db))
                        .ToList();
                }
                return databases;
            }

            public static Db Lookup(string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List    /* uses default server, credentials */
                (
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<Db>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Database not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            internal static Db Lookup(out SqlConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var server = SqlDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                var filteredList = List
                (
                    out conn,
                    server,
                    credentials: SqlDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<Db>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Database not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static Db Lookup(SqlConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<Db>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Database not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static Db Lookup(SqlDbConnectionInfo connectionInfo, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var server = connectionInfo.DataSource ?? throw new UtilityException("DataSource cannot be null");
                return Lookup
                (
                    server,
                    name,
                    credentials: connectionInfo.Credentials,
                    ignoreCase: ignoreCase,
                    commandTimeout: commandTimeout
                );
            }

            public static Db Lookup(DbServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup
                    (
                        conn,
                        server,
                        name,
                        ignoreCase: ignoreCase,
                        commandTimeout: commandTimeout
                    );
                }
            }

            internal static Db Lookup(out SqlConnection conn, DbServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    out conn,
                    server,
                    credentials: credentials ?? SqlDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<Db>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Database not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static Db Lookup(SqlConnection conn, DbServer server, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    server,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<Db>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Database not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }
        }

        public static class Objects
        {
            public static IEnumerable<DbObject> List(string dbName, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(out SqlConnection conn, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);    /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, database, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> List(SqlConnection conn, string dbName, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(conn, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, database, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbObject> List(DbServer server, Credential? credentials = null, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> List(SqlConnection conn, DbServer server, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                return List(conn, server, objectType, filter, commandTimeout);
            }

            public static IEnumerable<DbObject> List(DbServer server, string dbName, Credential? credentials = null, bool ignoreCase = false, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, dbName, ignoreCase: ignoreCase, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> List(SqlConnection conn, DbServer server, string dbName, bool ignoreCase = false, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                var database = Databases.Lookup(conn, server, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, database, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbObject> List(Db database, Credential? credentials = null, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = (database ?? throw new UtilityException("Database cannot be null")).Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return List(conn, database, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> List(SqlConnection conn, Db database, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? commandTimeout = null)
            {
                var typeCondition = objectType.HasValue
                    ? "RTRIM([type]) = '" + SqlServerTypes.ResolveType(objectType.Value) + "'"
                    : "RTRIM([type]) IN ('U', 'V', 'P', 'FN', 'TF', 'IF')";
                string statement = @"
                    USE " + database + @"

                    SELECT 
	                    SCHEMA_NAME([schema_id]) AS [schema], 
	                    [name], 
	                    RTRIM([type]) AS [type], 
	                    [type_desc]
                    FROM 
	                    [sys].[all_objects]
                    WHERE 
	                    [is_ms_shipped] = 0 AND
	                    [name] NOT LIKE '$ndo%' AND
	                    " + typeCondition + @"
                    ORDER BY 
	                    CASE RTRIM([type])
		                    WHEN 'U'  THEN 1     -- U = Table
		                    WHEN 'V'  THEN 2     -- V = View
		                    WHEN 'P'  THEN 3     -- P = Stored Procedure
		                    WHEN 'FN' THEN 4     -- FN = Function
		                    WHEN 'TF' THEN 5     -- TF = Table-valued Function
		                    WHEN 'IF' THEN 6     -- IF = Inline Table-valued Function
	                    END,
	                    [name]
                ";
                var objects = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.From((reader) => new DbObject
                    (
                        reader["name"] as string,
                        SqlServerTypes.LookupObjectType(reader["type"] as string)
                    )
                    {
                        Parent = new DbSchema(reader["schema"] as string)
                        {
                            Parent = database
                        }
                    }),
                    commandTimeout: commandTimeout
                );
                if (filter != null)
                {
                    return objects
                        .Where(o => filter.Invoke(o))
                        .ToList();
                }
                return objects;
            }

            public static DbObject Lookup(string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                return Lookup(out _, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);   /* uses default server, credentials */
            }

            internal static DbObject Lookup(out SqlConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var server = SqlDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server, Credentials = SqlDbSettings.DefaultCredentials });
                var filteredList = List    /* uses default server, credentials */
                (
                    conn,
                    server,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<DbObject>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name);
                }
            }

            public static DbObject Lookup(SqlConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var server = new DbServer(conn.DataSource);
                return Lookup(conn, server, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(string dbName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(out SqlConnection conn, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default server, credentials */
                using (conn)
                {
                    return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            internal static DbObject Lookup(out SqlConnection conn, string dbName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(out conn, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default server, credentials */
                return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(SqlConnection conn, string dbName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(conn, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(DbServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                return Lookup(server, name, credentials: credentials, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(SqlConnection conn, DbServer server, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                    (
                    conn,
                    server,
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<DbObject>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name + " (hint: lookup also by Database)");
                }
            }

            public static DbObject Lookup(DbServer server, string dbName, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(out SqlConnection conn, server, dbName, credentials: credentials, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(SqlConnection conn, DbServer server, string dbName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var database = Databases.Lookup(conn, server, dbName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbObject Lookup(Db database, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = database.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return Lookup(conn, database, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static DbObject Lookup(SqlConnection conn, Db database, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    database,
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<DbObject>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name + " (this should never happen)");
                }
            }

            public static IEnumerable<DbObject> SearchByName(Db database, IComparator<string> comparator, SqlObjectType? objectType = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = database.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return SearchByName(conn, database, comparator, objectType: objectType, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> SearchByName(SqlConnection conn, Db database, ICriterinator<string> comparator, SqlObjectType? objectType = null, int? commandTimeout = null)
            {
                var objects = List(conn, database, objectType: objectType, commandTimeout: commandTimeout);
                objects = objects
                    .Where(obj => comparator.IsMatch(obj.Name))
                    .ToList();
                return objects;
            }

            public static IEnumerable<DbObject> SearchByName(Db database, string partialName, bool ignoreCase = false, SqlObjectType? objectType = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = database.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return SearchByName(conn, database, partialName, ignoreCase: ignoreCase, objectType: objectType, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbObject> SearchByName(SqlConnection conn, Db database, string partialName, bool ignoreCase = false, SqlObjectType? objectType = null, int? commandTimeout = null)
            {
                var comparator = Comparator.Contains(partialName, ignoreCase: ignoreCase);
                return SearchByName(conn, database, comparator, objectType: objectType, commandTimeout: commandTimeout);
            }
        }

        public static class Columns
        {
            public static IEnumerable<DbColumn> List(string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out SqlConnection conn, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> List(SqlConnection conn, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> List(string dbName, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out SqlConnection conn, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> List(SqlConnection conn, string dbName, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> List(DbServer server, string dbName, string tableOrViewName, Credential? credentials = null, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, dbName, tableOrViewName, ignoreCase: ignoreCase, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> List(SqlConnection conn, DbServer server, string dbName, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, server, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> List(Db database, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = database.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return List(conn, database, tableOrViewName, ignoreCase: ignoreCase, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> List(SqlConnection conn, Db database, string tableOrViewName, bool ignoreCase = false, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, database, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> List(DbObject tableOrView, Predicate<DbColumn> filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return List(conn, tableOrView, filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> List(SqlConnection conn, DbObject obj, Predicate<DbColumn> filter = null, int? commandTimeout = null)
            {
                var conditions = new List<string>
                {
                    "O.[name] = '" + obj.Name + "'"
                };

                if (obj.Schema != null)
                {
                    conditions.Add("SCHEMA_NAME(O.[schema_id]) = '" + obj.Schema.Name + "'");
                }

                var statement = @"
                    USE " + (obj.Database ?? throw new UtilityException("No Database ancestor exists for the supplied " + (obj?.ObjectType.ToString() ?? "Object"))) + @"
                    SELECT 
                        SCHEMA_NAME(O.[schema_id]) AS [schema_name], 
                        O.[name] AS [object_name], 
                        RTRIM(O.[type]) AS [object_type], 
                        C.[name] AS [column_name], 
                        T.[name] AS [column_type], 
                        CAST
                        (
                            CASE 
    	                        WHEN C.[user_type_id] IN (239, 231) THEN C.[max_length] / 2   /* 239 = nchar, 231 = nvarchar */
    	                        WHEN C.[user_type_id] IN (104, 48, 52, 56, 127) THEN 0        /* 104 = bit, 48 = tinyint, 52 = smallint, 56 = int, 127 = bigint */
    	                        WHEN C.[user_type_id] IN (106, 62, 59) THEN 0                 /* 106 = decimal, 62 = float, 59 = real */
    	                        WHEN C.[user_type_id] IN (122, 60) THEN 0                     /* 122 = smallmoney, 60 = money */
    	                        WHEN C.[user_type_id] IN (58, 61) THEN 0                      /* 58 = smalldatetime, 61 = datetime */
    	                        ELSE C.[max_length]
                            END AS smallint
                        ) AS [column_length], 
                        C.[precision], 
                        C.[scale], 
                        C.[is_nullable], 
                        C.[max_length],
                        C.[user_type_id],
                        O.[object_id],
                        C.[column_id]
                    FROM 
                        [sys].[columns] AS C 
                    INNER JOIN 
                        [sys].[objects] AS O 
                        ON C.[object_id] = O.[object_id]
                    LEFT JOIN 
                        [sys].[types] AS T 
                        ON T.[user_type_id] = C.[user_type_id]
                    WHERE 
                        O.[is_ms_shipped] = 0 AND
                        O.[name] NOT LIKE '$%' AND
                        " + string.Join(" AND ", conditions) + @"
                    ORDER BY 
                        C.[column_id]
                ";
                var columns = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.From((reader) => new DbColumn
                    (
                        reader["column_name"] as string,
                        SqlServerTypes.LookupColumnType((int)reader["user_type_id"])
                    )
                    {
                        Parent = obj,
                        Length = (short)reader["column_length"],
                        Precision = (byte)reader["precision"],
                        Scale = (byte)reader["scale"],
                        IsNullable = (bool)reader["is_nullable"],
                        MaxLength = (short)reader["max_length"],
                        ColumnID = (int)reader["column_id"]
                    }),
                    commandTimeout: commandTimeout
                );
                if (filter != null)
                {
                    columns = columns.Where(c => filter.Invoke(c)).ToList();
                }
                return columns;
            }

            public static DbColumn Lookup(string dbName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out SqlConnection conn, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);      /* uses default server, credentials */
                using (conn)
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static DbColumn Lookup(SqlConnection conn, string dbName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbColumn Lookup(DbServer server, string dbName, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup(conn, server, dbName, tableOrViewName, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static DbColumn Lookup(SqlConnection conn, DbServer server, string dbName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, server, dbName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbColumn Lookup(Db database, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = database.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Database"), Credentials = credentials }))
                {
                    return Lookup(conn, database, tableOrViewName, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static DbColumn Lookup(SqlConnection conn, Db database, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, database, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static DbColumn Lookup(DbObject tableOrView, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static DbColumn Lookup(SqlConnection conn, DbObject tableOrView, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    tableOrView,
                    filter: (c) => c.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                ) as List<DbColumn>;
                switch (filteredList.Count)
                {
                    case 0:
                        throw new UtilityException("Column not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple columns found: " + name + " (this should never happen)");
                }
            }

            public static IEnumerable<DbColumn> SearchByName(DbObject obj, ICriterinator<string> comparator, Credential? credentials = null, int ? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = obj.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (obj?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, obj, comparator, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> SearchByName(SqlConnection conn, DbObject obj, ICriterinator<string> comparator, int? commandTimeout = null)
            {
                var columns = List(conn, obj, commandTimeout: commandTimeout);
                columns = columns
                    .Where(col => comparator.IsMatch(col.Name))
                    .ToList();
                return columns;
            }

            public static IEnumerable<DbColumn> SearchByName(DbObject obj, string partialName, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = obj.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (obj?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, obj, partialName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> SearchByName(SqlConnection conn, DbObject obj, string partialName, bool ignoreCase = false, int? commandTimeout = null)
            {
                var comparator = Comparator.Contains
                (
                    partialName,
                    ignoreCase: ignoreCase
                );
                return SearchByName(conn, obj, comparator, commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> SearchByValue(DbObject obj, object value, Predicate<DbColumn> filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = obj.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (obj?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByValue(conn, obj, value, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> SearchByValue(SqlConnection conn, DbObject obj, object value, Predicate<DbColumn> filter = null, int? commandTimeout = null, Action<string> columnSearched = null)
            {
                var columns = List(conn, obj, filter: filter, commandTimeout: commandTimeout);
                IEnumerable<object> dictinctValues;
                var matchingColumns = new List<DbColumn>();
                foreach (var column in columns)
                {
                    //if (DoesColumnContainValue(conn, column, value, commandTimeout))
                    //{
                    //    matchingColumns.Add(column);
                    //}
                    dictinctValues = GetDistinctValues(conn, column, commandTimeout);
                    if (value is Comparator<string> stringComparator)
                    {
                        foreach (object dictinctValue in dictinctValues)
                        {
                            if (stringComparator.IsMatch(dictinctValue?.ToString()))
                            {
                                matchingColumns.Add(column);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (object dictinctValue in dictinctValues)
                        {
                            if (Equals(value, dictinctValue))
                            {
                                matchingColumns.Add(column);
                                break;
                            }
                        }
                    }
                    columnSearched?.Invoke(column.ToFullyQualifiedString(startingWith: SqlObjectType.Database));
                }
                return matchingColumns;
            }

            public static IEnumerable<DbColumn> SearchTextColumnsByValue(DbObject obj, string value, bool ignoreCase = false, int? commandTimeout = null)
            {
                return SearchTextColumnsByCriteria(obj, Comparator.Equals(value, ignoreCase: ignoreCase), commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> SearchTextColumnsByValue(SqlConnection conn, DbObject obj, string value, bool ignoreCase = false, int? commandTimeout = null)
            {
                return SearchTextColumnsByCriteria(conn, obj, Comparator.Equals(value, ignoreCase: ignoreCase), commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> SearchTextColumnsByCriteria(DbObject obj, ICriterinator<string> comparator, int? commandTimeout = null)
            {
                return SearchByValue(obj, comparator, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
            }

            public static IEnumerable<DbColumn> SearchTextColumnsByCriteria(SqlConnection conn, DbObject obj, ICriterinator<string> comparator, int? commandTimeout = null)
            {
                return SearchByValue(conn, obj, comparator, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
            }

            private static IEnumerable<object> GetDistinctValues(SqlConnection conn, DbColumn column, int? commandTimeout)
            {
                if (column.Parent == null)
                {
                    throw new UtilityException("Table or view not specified");
                }
                var conditions = new List<string>();
                if (column.IsNullable)
                {
                    conditions.Add(column + " IS NOT NULL");
                }
                var statement = @"
                    USE " + (column.Database ?? throw new UtilityException("No Database ancestor exists for the supplied Column")) + @"
                    SELECT DISTINCT " + column + @" 
                    FROM " + column.Parent.ToFullyQualifiedString(SqlObjectType.Database) + @"
                    " + (conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "");
                var distinctValues = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.From((object[] objects) => objects[0]),
                    commandTimeout: commandTimeout,
                    autoTrunc: AutoTruncate.Zap
                );
                return distinctValues;
            }

            //private static bool DoesColumnContainValue(SqlConnection conn, DbColumn column, object value, int? commandTimeout)
            //{
            //    var statement = @"
            //        USE " + (column.Database ?? throw new UtilityException("No Database ancestor exists for the supplied Column")) + @"
            //        SELECT TOP 1 " + column + @" 
            //        FROM " + column.Parent.ToFullyQualifiedString(SqlObjectType.Database) + @"
            //        WHERE ";
            //    Func<object, bool> eval = (_result) => _result != null && Equals(_result, value);
            //    if (value is SearchCriteria searchCriteria)
            //    {
            //        switch (searchCriteria.Comparison)
            //        {
            //            case Comparison.Equals:
            //                statement += "UPPER(" + column + ") = '" + searchCriteria.SearchTerm.ToUpper() + "'";
            //                break;
            //            case Comparison.NotEquals:
            //                statement += "UPPER(" + column + ") <> '" + searchCriteria.SearchTerm.ToUpper() + "'";
            //                break;
            //            case Comparison.Contains:
            //                statement += "UPPER(" + column + ") LIKE '%" + searchCriteria.SearchTerm.ToUpper() + "%'";
            //                break;
            //            case Comparison.NotContains:
            //                statement += "UPPER(" + column + ") NOT LIKE '%" + searchCriteria.SearchTerm.ToUpper() + "%'";
            //                break;
            //            case Comparison.StartsWith:
            //                statement += "UPPER(" + column + ") LIKE '" + searchCriteria.SearchTerm.ToUpper() + "%'";
            //                break;
            //            case Comparison.NotStartsWith:
            //                statement += "UPPER(" + column + ") NOT LIKE '" + searchCriteria.SearchTerm.ToUpper() + "%'";
            //                break;
            //            case Comparison.EndsWith:
            //                statement += "UPPER(" + column + ") LIKE '%" + searchCriteria.SearchTerm.ToUpper() + "'";
            //                break;
            //            case Comparison.NotEndsWith:
            //                statement += "UPPER(" + column + ") NOT LIKE '%" + searchCriteria.SearchTerm.ToUpper() + "'";
            //                break;
            //            default:
            //                throw new UtilityException("Comparison is currently not supported: " + searchCriteria.Comparison);
            //        }
            //        eval = (_result) => _result != null && searchCriteria.Evaluate(_result?.ToString());
            //    }
            //    else
            //    {
            //        statement += column + " = " + DbUtil.Sqlize(value);
            //    }
            //    var result = Query.SQL.AsScalar(conn, statement, commandTimeout: commandTimeout);
            //    return eval.Invoke(result);
            //}

            public static IEnumerable<DbColumn> SearchTextColumnsByHasNonprintableCharacters(DbObject obj, bool distinct = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = obj.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (obj?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchTextColumnsByHasNonprintableCharacters(conn, obj, distinct: distinct, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<DbColumn> SearchTextColumnsByHasNonprintableCharacters(SqlConnection conn, DbObject obj, bool distinct = false, int? commandTimeout = null)
            {
                var columns = List(conn, obj, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
                IEnumerable<string> nonPrintValues;
                var matchingColumns = new List<DbColumn>();
                foreach (var column in columns)
                {
                    nonPrintValues = GetValuesWithNonprintableCharacters(conn, column, distinct, commandTimeout);
                    if (nonPrintValues.Any())
                    {
                        matchingColumns.Add(column);
                    }
                }
                return matchingColumns;
            }

            public static IEnumerable<string> GetValuesWithNonprintableCharacters(DbColumn column, bool distinct = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = SqlDbUtil.LaunchConnection(connectionInfo: new SqlDbConnectionInfo { DataSource = column.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Column"), Credentials = credentials }))
                {
                    conn.Open();
                    return GetValuesWithNonprintableCharacters(conn, column, distinct: distinct, commandTimeout: commandTimeout);
                }
            }

            private static IEnumerable<string> GetValuesWithNonprintableCharacters(SqlConnection conn, DbColumn column, bool distinct, int? commandTimeout)
            {
                if (column.Parent == null)
                {
                    throw new UtilityException("Table or view not specified");
                }
                var conditions = new List<string>();
                if (column.IsNullable)
                {
                    conditions.Add(column + " IS NOT NULL");
                }
                conditions.Add
                (
                    @"
                    (
	                    " + column + @" LIKE '%' + char(0) + '%' OR
	                    " + column + @" LIKE '%' + char(1) + '%' OR
	                    " + column + @" LIKE '%' + char(2) + '%' OR
	                    " + column + @" LIKE '%' + char(3) + '%' OR
	                    " + column + @" LIKE '%' + char(4) + '%' OR
	                    " + column + @" LIKE '%' + char(5) + '%' OR
	                    " + column + @" LIKE '%' + char(6) + '%' OR
	                    " + column + @" LIKE '%' + char(7) + '%' OR
	                    " + column + @" LIKE '%' + char(9) + '%' OR
	                    " + column + @" LIKE '%' + char(10) + '%' OR
	                    " + column + @" LIKE '%' + char(13) + '%'
                    )"
                );
                var statement = @"
                    USE " + (column.Database ?? throw new UtilityException("No Database ancestor exists for the supplied Column")) + @"
                    SELECT " + (distinct ? "DISTINCT " : "") + column + @"
                    FROM " + column.Parent.ToFullyQualifiedString(SqlObjectType.Database) + @"
                    WHERE " + string.Join(" AND ", conditions);
                var values = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.ScalarString,
                    commandTimeout: commandTimeout
                );
                return values;
            }
        }
    }
}
