using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb.Meta
{
    public static class MetaSearch
    {
        public static class Schemas
        {
            public static IEnumerable<OraSchema> List(Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                var server = OracleDbSettings.DefaultServer ?? new OraServer(OracleDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings"));
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server, Credentials = OracleDbSettings.DefaultCredentials }))
                {
                    return List(conn, server, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraSchema> List(OracleConnection conn, Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                var server = new OraServer(conn.DataSource, serviceName: conn.ServiceName, instanceName: conn.InstanceName);
                return List(conn, server, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraSchema> List(OracleDbConnectionInfo connectionInfo, Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                var server = connectionInfo.Server ?? new OraServer(connectionInfo.DataSource ?? throw new UtilityException("Server and DataSource cannot both be null"));
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: connectionInfo))
                {
                    return List(conn, server, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraSchema> List(OraServer server, Credential? credentials = null, Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, filter: filter, timeout: timeout);
                }
            }

            internal static IEnumerable<OraSchema> List(out OracleConnection conn, OraServer server, Credential? credentials = null, Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials });
                return List(conn, server, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraSchema> List(OracleConnection conn, OraServer server, Predicate<OraSchema>? filter = null, int? timeout = null)
            {
                string statement = @"
                    SELECT USERNAME AS ""schema""
                    FROM DBA_USERS
                ";
                var schemas = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    readerParser: (reader) => new OraSchema(reader["schema"] as string) { Parent = server },
                    autoSort: new AutoSort<OraSchema>(s => s.Name),
                    timeout: timeout
                );
                if (filter != null)
                {
                    return schemas
                        .Where(s => filter.Invoke(s))
                        .ToList();
                }
                return schemas;
            }

            public static OraSchema Lookup(string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List    /* uses default server, credentials */
                (
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Schema not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            internal static OraSchema Lookup(out OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
            {
                var server = OracleDbSettings.DefaultServer ?? new OraServer(OracleDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings"));
                var filteredList = List
                (
                    out conn,
                    server,
                    credentials: OracleDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Schema not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static OraSchema Lookup(OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                (
                    conn,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Schema not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static OraSchema Lookup(OracleDbConnectionInfo connectionInfo, string name, bool ignoreCase = false, int? timeout = null)
            {
                var server = connectionInfo.Server ?? new OraServer(connectionInfo.DataSource ?? throw new UtilityException("Server and DataSource cannot both be null"));
                return Lookup
                (
                    server,
                    name,
                    credentials: connectionInfo.Credentials,
                    ignoreCase: ignoreCase,
                    timeout: timeout
                );
            }

            public static OraSchema Lookup(OraServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup
                    (
                        conn,
                        server,
                        name,
                        ignoreCase: ignoreCase,
                        timeout: timeout
                    );
                }
            }

            internal static OraSchema Lookup(out OracleConnection conn, OraServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                (
                    out conn,
                    server,
                    credentials: credentials ?? OracleDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Schema not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }

            public static OraSchema Lookup(OracleConnection conn, OraServer server, string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                (
                    conn, 
                    server, 
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Schema not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple schemas found: " + name + " (this should never happen)");
                }
            }
        }

        public static class Objects
        {
            public static IEnumerable<OraObject> List(string schemaName, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, schemaName, ignoreCase: ignoreCase, timeout: timeout);    /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, schema, objectType: objectType, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, string schemaName, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(conn, schemaName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, schema, objectType: objectType, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraObject> List(OraServer server, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, objectType: objectType, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraServer server, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                return List(conn, server, null, objectType, filter, timeout);
            }

            public static IEnumerable<OraObject> List(OraServer server, string schemaName, Credential? credentials = null, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, schemaName, ignoreCase: ignoreCase, objectType: objectType, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraServer server, string schemaName, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                var schema = Schemas.Lookup(conn, server, schemaName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, schema, objectType: objectType, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraObject> List(OraSchema schema, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = (schema ?? throw new UtilityException("Schema cannot be null")).Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return List(conn, schema, objectType: objectType, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraSchema schema, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? timeout = null)
            {
                return List(conn, null, schema, objectType, filter, timeout);
            }

            private static IEnumerable<OraObject> List(OracleConnection conn, OraServer? server, OraSchema? schema, OraObjectType? objectType, Predicate<OraObject>? filter, int? timeout)
            {
                if (server == null)
                {
                    if (schema == null) throw new UtilityException("Server and Schema cannot both be null");
                }
                else if(schema != null) throw new UtilityException("Please supply either the Server or Schema, not both");

                var conditions = new List<string>();
                if (schema != null)
                {
                    conditions.Add("OWNER = '" + schema.Name + "'");
                }
                conditions.Add
                (
                    objectType.HasValue
                        ? "OBJECT_TYPE = '" + OracleTypes.ResolveObjectType(objectType.Value) + "'"
                        : "OBJECT_TYPE IN ('TABLE', 'VIEW', 'MATERIALIZED VIEW', 'PROCEDURE', 'FUNCTION')"
                );
                string sql = @"
                    SELECT OWNER AS ""schema"", OBJECT_NAME AS ""name"", OBJECT_TYPE AS ""type""
                    FROM ALL_OBJECTS
                    WHERE " + string.Join(" AND ", conditions);
                var objects = Query.SQL.AsCollection
                (
                    conn,
                    sql,
                    readerParser: (reader) => 
                        new OraObject
                        (
                            reader["name"] as string,
                            OracleTypes.LookupObjectType(reader["type"] as string)
                        )
                        {
                            Parent = schema ?? new OraSchema(reader["schema"] as string) { Parent = server }
                        },
                    autoSort: new AutoSort<OraObject>(o => o.Schema.Name + o.Name),
                    timeout: timeout
                );
                if (filter != null)
                {
                    return objects
                        .Where(o => filter.Invoke(o))
                        .ToList();
                }
                return objects;
            }

            public static OraObject Lookup(string name, bool ignoreCase = false, int? timeout = null)
            {
                return Lookup(out _, name, ignoreCase: ignoreCase, timeout: timeout);   /* uses default server, credentials */
            }

            internal static OraObject Lookup(out OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
            {
                var server = OracleDbSettings.DefaultServer ?? new OraServer(OracleDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings"));
                conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server, Credentials = OracleDbSettings.DefaultCredentials });
                var filteredList = List    /* uses default server, credentials */
                (
                    conn,
                    server,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                ) as List<OraObject>;
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

            public static OraObject Lookup(OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
            {
                var server = new OraServer(conn.DataSource, serviceName: conn.ServiceName, instanceName: conn.InstanceName);
                return Lookup(conn, server, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(string schemaName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, schemaName, ignoreCase: ignoreCase, timeout: timeout);     /* uses default server, credentials */
                using (conn)
                {
                    return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            internal static OraObject Lookup(out OracleConnection conn, string schemaName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(out conn, schemaName, ignoreCase: ignoreCase, timeout: timeout);     /* uses default server, credentials */
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(OracleConnection conn, string schemaName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(conn, schemaName, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(OraServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                return Lookup(server, name, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(OracleConnection conn, OraServer server, string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                    (
                    conn,
                    server,
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name + " (hint: lookup also by Schema)");
                }
            }

            public static OraObject Lookup(OraServer server, string schemaName, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, server, schemaName, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(OracleConnection conn, OraServer server, string schemaName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var schema = Schemas.Lookup(conn, server, schemaName, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraObject Lookup(OraSchema schema, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static OraObject Lookup(OracleConnection conn, OraSchema schema, string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                (
                    conn, 
                    schema, 
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name + " (this should never happen)");
                }
            }

            public static IEnumerable<OraObject> SearchByName(OraSchema schema, SearchCriteria criteria, OraObjectType? objectType = null, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return SearchByName(conn, schema, criteria, objectType: objectType, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> SearchByName(OracleConnection conn, OraSchema schema, SearchCriteria criteria, OraObjectType? objectType = null, int? timeout = null)
            {
                var objects = List(conn, schema, objectType: objectType, timeout: timeout);
                objects = objects
                    .Where(obj => criteria.Evaluate(obj.Name))
                    .ToList();
                return objects;
            }

            public static IEnumerable<OraObject> SearchByName(OraSchema schema, string partialName, bool ignoreCase = false, OraObjectType? objectType = null, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return SearchByName(conn, schema, partialName, ignoreCase: ignoreCase, objectType: objectType, timeout: timeout);
                }
            }

            public static IEnumerable<OraObject> SearchByName(OracleConnection conn, OraSchema schema, string partialName, bool ignoreCase = false, OraObjectType? objectType = null, int? timeout = null)
            {
                var searchCriteria = SearchCriteria.Contains(partialName, ignoreCase: ignoreCase);
                return SearchByName(conn, schema, searchCriteria, objectType: objectType, timeout: timeout);
            }
        }

        public static class Columns
        {
            public static IEnumerable<OraColumn> List(string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);     /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, tableOrView, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraColumn> List(string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);     /* uses default server, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, tableOrView, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraColumn> List(OraServer server, string schemaName, string tableOrViewName, Credential? credentials = null, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, server, schemaName, tableOrViewName, ignoreCase: ignoreCase, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraServer server, string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, server, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, tableOrView, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraColumn> List(OraSchema schema, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return List(conn, schema, tableOrViewName, ignoreCase: ignoreCase, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraSchema schema, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schema, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return List(conn, tableOrView, filter: filter, timeout: timeout);
            }

            public static IEnumerable<OraColumn> List(OraObject tableOrView, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return List(conn, tableOrView, filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraObject tableOrView, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var conditions = new List<string>();
                if (tableOrView.Schema != null)
                {
                    conditions.Add("OWNER = '" + tableOrView.Schema + "'");
                }
                conditions.Add("TABLE_NAME = '" + tableOrView.Name + "'");

                string sql = @"
                    SELECT 
	                    OWNER AS ""schema"", 
	                    COLUMN_NAME AS ""name"", 
	                    DATA_TYPE AS ""type"",
	                    DATA_LENGTH AS ""length"",
	                    DATA_PRECISION AS ""precision"",
	                    DATA_SCALE AS ""scale"",
	                    NULLABLE AS ""isNullable"",
	                    COLUMN_ID AS ""columnID"",
	                    DATA_DEFAULT AS ""defaultValue""
                    FROM 
	                    ALL_TAB_COLUMNS
                    WHERE 
	                    " + string.Join(" AND ", conditions) + @"
                    ORDER BY 
	                    COLUMN_ID
                ";

                var columns = Query.SQL.AsCollection
                (
                    conn,
                    sql,
                    readerParser: (reader) => new OraColumn
                    (
                        name: reader["name"] as string,
                        columnType: OracleTypes.LookupColumnType(reader["type"] as string)
                    )
                    {
                        Parent = tableOrView.Schema != null ? tableOrView : tableOrView.SetSchemaByName(reader["schema"] as string),
                        Length = Zap.Int(reader["length"]),
                        Precision = Zap.NInt(reader["precision"]),
                        Scale = Zap.NInt(reader["scale"]),
                        IsNullable = Zap.Bool(reader["isNullable"]),
                        ColumnID = Zap.Int(reader["columnID"]),
                        DefaultValue = Zap.String(reader["defaultValue"])
                    },
                    timeout: timeout
                );

                if (filter != null)
                {
                    return columns.Where(c => filter.Invoke(c)).ToList();
                }
                return columns;
            }

            public static OraColumn Lookup(string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);      /* uses default server, credentials */
                using (conn)
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraColumn Lookup(OraServer server, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = server ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup(conn, server, schemaName, tableOrViewName, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraServer server, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, server, schemaName, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraColumn Lookup(OraSchema schema, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return Lookup(conn, schema, tableOrViewName, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraSchema schema, string tableOrViewName, string name, bool ignoreCase = false, int? timeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schema, tableOrViewName, ignoreCase: ignoreCase, timeout: timeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, timeout: timeout);
            }

            public static OraColumn Lookup(OraObject tableOrView, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraObject tableOrView, string name, bool ignoreCase = false, int? timeout = null)
            {
                var filteredList = List
                (
                    conn,
                    tableOrView,
                    filter: (c) => c.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    timeout: timeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Column not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple columns found: " + name + " (this should never happen)");
                }
            }

            public static IEnumerable<OraColumn> SearchByName(OraObject tableOrView, SearchCriteria searchCriteria, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, tableOrView, searchCriteria, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByName(OracleConnection conn, OraObject tableOrView, SearchCriteria searchCriteria, int? timeout = null)
            {
                var columns = List(conn, tableOrView, timeout: timeout);
                columns = columns
                    .Where(col => searchCriteria.Evaluate(col.Name))
                    .ToList();
                return columns;
            }

            public static IEnumerable<OraColumn> SearchByName(OraObject tableOrView, string partialName, bool ignoreCase = false, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, tableOrView, partialName, ignoreCase: ignoreCase, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByName(OracleConnection conn, OraObject tableOrView, string partialName, bool ignoreCase = false, int? timeout = null)
            {
                var searchCriteria = SearchCriteria.Contains(partialName, ignoreCase: ignoreCase);
                return SearchByName(conn, tableOrView, searchCriteria, timeout: timeout);
            }

            public static IEnumerable<OraColumn> SearchByValue(OraObject tableOrView, object value, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByValue(conn, tableOrView, value, filter: filter, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByValue(OracleConnection conn, OraObject tableOrView, object value, Predicate<OraColumn>? filter = null, int? timeout = null)
            {
                var columns = List(conn, tableOrView, filter: filter, timeout: timeout);
                IEnumerable<object?> dictinctValues;
                var matchingColumns = new List<OraColumn>();
                foreach (var column in columns)
                {
                    dictinctValues = GetDistinctValues(conn, column, timeout);
                    if (value is SearchCriteria searchCriteria)
                    {
                        foreach (object? dictinctValue in dictinctValues)
                        {
                            if (searchCriteria.Evaluate(dictinctValue?.ToString()))
                            {
                                matchingColumns.Add(column);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (object? dictinctValue in dictinctValues)
                        {
                            if (Equals(value, dictinctValue))
                            {
                                matchingColumns.Add(column);
                                break;
                            }
                        }
                    }
                }
                return matchingColumns;
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByValue(OraObject tableOrView, string value, bool ignoreCase = false, int? timeout = null)
            {
                return SearchTextColumnsByCriteria(tableOrView, new SearchCriteria(value, Comparison.Equals, ignoreCase: ignoreCase), timeout: timeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByValue(OracleConnection conn, OraObject tableOrView, string value, bool ignoreCase = false, int? timeout = null)
            {
                return SearchTextColumnsByCriteria(conn, tableOrView, new SearchCriteria(value, Comparison.Equals, ignoreCase: ignoreCase), timeout: timeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByCriteria(OraObject tableOrView, SearchCriteria criteria, int? timeout = null)
            {
                return SearchByValue(tableOrView, criteria, filter: (column) => column.IsText(), timeout: timeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByCriteria(OracleConnection conn, OraObject tableOrView, SearchCriteria criteria, int? timeout = null)
            {
                return SearchByValue(conn, tableOrView, criteria, filter: (column) => column.IsText(), timeout: timeout);
            }

            private static IEnumerable<object?> GetDistinctValues(OracleConnection conn, OraColumn column, int? timeout)
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
                var sql = @"
                    SELECT DISTINCT " + column + @" 
                    FROM " + column.Parent.ToFullyQualifiedString(OraObjectType.Schema) + @"
                    " + (conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "");
                var distinctValues = Query.SQL.AsCollection
                (
                    conn,
                    sql,
                    objectParser: (objects) => objects[0],
                    timeout: timeout,
                    autoTrunc: AutoTruncate.Zap
                );
                return distinctValues;
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByHasNonprintableCharacters(OraObject tableOrView, bool distinct = false, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchTextColumnsByHasNonprintableCharacters(conn, tableOrView, distinct: distinct, timeout: timeout);
                }
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByHasNonprintableCharacters(OracleConnection conn, OraObject tableOrView, bool distinct = false, int? timeout = null)
            {
                var columns = List(conn, tableOrView, filter: (column) => column.IsText(), timeout: timeout);
                IEnumerable<string?> nonPrintValues;
                var matchingColumns = new List<OraColumn>();
                foreach (var column in columns)
                {
                    nonPrintValues = GetValuesWithNonprintableCharacters(conn, column, distinct, timeout);
                    if (nonPrintValues.Any())
                    {
                        matchingColumns.Add(column);
                    }
                }
                return matchingColumns;
            }

            public static IEnumerable<string?> GetValuesWithNonprintableCharacters(OraColumn column, bool distinct = false, Credential? credentials = null, int? timeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { Server = column.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Column"), Credentials = credentials }))
                {
                    conn.Open();
                    return GetValuesWithNonprintableCharacters(conn, column, distinct: distinct, timeout: timeout);
                }
            }

            private static IEnumerable<string?> GetValuesWithNonprintableCharacters(OracleConnection conn, OraColumn column, bool distinct, int? timeout)
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
                        INSTR(" + column + @", chr(0)) != 0 OR
                        INSTR(" + column + @", chr(1)) != 0 OR
                        INSTR(" + column + @", chr(2)) != 0 OR
                        INSTR(" + column + @", chr(3)) != 0 OR
                        INSTR(" + column + @", chr(4)) != 0 OR
                        INSTR(" + column + @", chr(5)) != 0 OR
                        INSTR(" + column + @", chr(6)) != 0 OR
                        INSTR(" + column + @", chr(7)) != 0 OR
                        INSTR(" + column + @", chr(8)) != 0 OR
                        INSTR(" + column + @", chr(9)) != 0 OR
                        INSTR(" + column + @", chr(10)) != 0 OR
                        INSTR(" + column + @", chr(13)) != 0
                    )"
                );
                var sql = @"
                    SELECT " + (distinct ? "DISTINCT " : "") + column + @" 
                    FROM " + column.Parent.ToFullyQualifiedString(OraObjectType.Schema) + @"
                    WHERE " + string.Join(" AND ", conditions);
                var nonPrintValues = Query.SQL.AsCollection
                (
                    conn,
                    sql,
                    readerParser: ScalarReaderParser.String,
                    timeout: timeout
                );
                return nonPrintValues;
            }
        }
    }
}
