using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Db;
using Horseshoe.NET.Primitives;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb.Meta
{
    public static class MetaSearch
    {
        public static class Schemas
        {
            public static IEnumerable<OraSchema> List(Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                var dataSource = OracleDbSettings.DefaultDataSource ?? throw new UtilityException("A default Server or DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource, Credentials = OracleDbSettings.DefaultCredentials }))
                {
                    return List(conn, dataSource, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraSchema> List(OracleConnection conn, Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                var dataSource = new OraServer(conn.DataSource, serviceName: conn.ServiceName, instanceName: conn.InstanceName);
                return List(conn, dataSource, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraSchema> List(OracleDbConnectionInfo connectionInfo, Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                var dataSource = connectionInfo.DataSource ?? throw new UtilityException("DataSource cannot be null");
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: connectionInfo))
                {
                    return List(conn, dataSource, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraSchema> List(OraServer dataSource, Credential? credentials = null, Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("DataSource cannot be null"), Credentials = credentials }))
                {
                    return List(conn, dataSource, filter: filter, commandTimeout: commandTimeout);
                }
            }

            internal static IEnumerable<OraSchema> List(out OracleConnection conn, OraServer dataSource, Credential? credentials = null, Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials });
                return List(conn, dataSource, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraSchema> List(OracleConnection conn, OraServer dataSource, Predicate<OraSchema>? filter = null, int? commandTimeout = null)
            {
                string statement = @"
                    SELECT USERNAME AS ""schema""
                    FROM DBA_USERS
                ";
                var schemas = Query.SQL.AsCollection
                (
                    conn,
                    statement,
                    rowParser: RowParser.From((reader) => new OraSchema(reader["schema"] as string) { Parent = dataSource }),
                    autoSort: new AutoSort<OraSchema>(s => s.Name),
                    commandTimeout: commandTimeout
                );
                if (filter != null)
                {
                    return schemas
                        .Where(s => filter.Invoke(s))
                        .ToList();
                }
                return schemas;
            }

            public static OraSchema Lookup(string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List    /* uses default dataSource, credentials */
                (
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            internal static OraSchema Lookup(out OracleConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var dataSource = OracleDbSettings.DefaultDataSource ?? throw new UtilityException("The default DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                var filteredList = List
                (
                    out conn,
                    dataSource,
                    credentials: OracleDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static OraSchema Lookup(OracleConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static OraSchema Lookup(OracleDbConnectionInfo connectionInfo, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var dataSource = connectionInfo.DataSource ?? throw new UtilityException("DataSource cannot be null");
                return Lookup
                (
                    dataSource,
                    name,
                    credentials: connectionInfo.Credentials,
                    ignoreCase: ignoreCase,
                    commandTimeout: commandTimeout
                );
            }

            public static OraSchema Lookup(OraServer dataSource, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup
                    (
                        conn,
                        dataSource,
                        name,
                        ignoreCase: ignoreCase,
                        commandTimeout: commandTimeout
                    );
                }
            }

            internal static OraSchema Lookup(out OracleConnection conn, OraServer dataSource, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    out conn,
                    dataSource,
                    credentials: credentials ?? OracleDbSettings.DefaultCredentials,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static OraSchema Lookup(OracleConnection conn, OraServer dataSource, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn, 
                    dataSource, 
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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
            public static IEnumerable<OraObject> List(string schemaName, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);    /* uses default dataSource, credentials */
                using (conn)
                {
                    return List(conn, schema, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, string schemaName, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(conn, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, schema, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraObject> List(OraServer dataSource, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, dataSource, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraServer dataSource, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                return List(conn, dataSource, null, objectType, filter, commandTimeout);
            }

            public static IEnumerable<OraObject> List(OraServer dataSource, string schemaName, Credential? credentials = null, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, dataSource, schemaName, ignoreCase: ignoreCase, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraServer dataSource, string schemaName, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(conn, dataSource, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, schema, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraObject> List(OraSchema schema, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = (schema ?? throw new UtilityException("Schema cannot be null")).Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return List(conn, schema, objectType: objectType, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> List(OracleConnection conn, OraSchema schema, OraObjectType? objectType = null, Predicate<OraObject>? filter = null, int? commandTimeout = null)
            {
                return List(conn, null, schema, objectType, filter, commandTimeout);
            }

            private static IEnumerable<OraObject> List(OracleConnection conn, OraServer? dataSource, OraSchema? schema, OraObjectType? objectType, Predicate<OraObject>? filter, int? commandTimeout)
            {
                if (dataSource == null)
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
                    rowParser: RowParser.From((reader) => 
                        new OraObject
                        (
                            reader["name"] as string,
                            OracleTypes.LookupObjectType(reader["type"] as string)
                        )
                        {
                            Parent = schema ?? new OraSchema(reader["schema"] as string) { Parent = dataSource }
                        }),
                    autoSort: new AutoSort<OraObject>(o => o.Schema.Name + o.Name),
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

            public static OraObject Lookup(string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                return Lookup(out _, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);   /* uses default dataSource, credentials */
            }

            internal static OraObject Lookup(out OracleConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var dataSource = OracleDbSettings.DefaultDataSource ?? throw new UtilityException("The default DataSource must be defined to use this method - see Settings or OrganizationDefaultSettings");
                conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource, Credentials = OracleDbSettings.DefaultCredentials });
                var filteredList = List    /* uses default dataSource, credentials */
                (
                    conn,
                    dataSource,
                    filter: (s) => s.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
                );
                switch (filteredList.Count())
                {
                    case 0:
                        throw new UtilityException("Object not found: " + name + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
                    case 1:
                        return filteredList.Single();
                    default:
                        throw new UtilityException("Multiple objects found: " + name);
                }
            }

            public static OraObject Lookup(OracleConnection conn, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var dataSource = new OraServer(conn.DataSource, serviceName: conn.ServiceName, instanceName: conn.InstanceName);
                return Lookup(conn, dataSource, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(string schemaName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default dataSource, credentials */
                using (conn)
                {
                    return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            internal static OraObject Lookup(out OracleConnection conn, string schemaName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(out conn, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default dataSource, credentials */
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(OracleConnection conn, string schemaName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(conn, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(OraServer dataSource, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                return Lookup(dataSource, name, credentials: credentials, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(OracleConnection conn, OraServer dataSource, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                    (
                    conn,
                    dataSource,
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static OraObject Lookup(OraServer dataSource, string schemaName, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(out OracleConnection conn, dataSource, schemaName, credentials: credentials, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(OracleConnection conn, OraServer dataSource, string schemaName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var schema = Schemas.Lookup(conn, dataSource, schemaName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraObject Lookup(OraSchema schema, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return Lookup(conn, schema, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static OraObject Lookup(OracleConnection conn, OraSchema schema, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn, 
                    schema, 
                    filter: (o) => o.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static IEnumerable<OraObject> SearchByName(OraSchema schema, IComparator<string> comparator, OraObjectType? objectType = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return SearchByName(conn, schema, comparator, objectType: objectType, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> SearchByName(OracleConnection conn, OraSchema schema, IComparator<string> comparator, OraObjectType? objectType = null, int? commandTimeout = null)
            {
                var objects = List(conn, schema, objectType: objectType, commandTimeout: commandTimeout);
                objects = objects
                    .Where(obj => comparator.IsMatch(obj.Name))
                    .ToList();
                return objects;
            }

            public static IEnumerable<OraObject> SearchByName(OraSchema schema, string partialName, bool ignoreCase = false, OraObjectType? objectType = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return SearchByName(conn, schema, partialName, ignoreCase: ignoreCase, objectType: objectType, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraObject> SearchByName(OracleConnection conn, OraSchema schema, string partialName, bool ignoreCase = false, OraObjectType? objectType = null, int? commandTimeout = null)
            {
                var comparator = Comparator.Contains
                (
                    partialName,
                    ignoreCase: ignoreCase
                ); 
                return SearchByName(conn, schema, comparator, objectType: objectType, commandTimeout: commandTimeout);
            }
        }

        public static class Columns
        {
            public static IEnumerable<OraColumn> List(string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default dataSource, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> List(string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);     /* uses default dataSource, credentials */
                using (conn)
                {
                    return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> List(OraServer dataSource, string schemaName, string tableOrViewName, Credential? credentials = null, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return List(conn, dataSource, schemaName, tableOrViewName, ignoreCase: ignoreCase, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraServer dataSource, string schemaName, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, dataSource, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> List(OraSchema schema, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return List(conn, schema, tableOrViewName, ignoreCase: ignoreCase, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraSchema schema, string tableOrViewName, bool ignoreCase = false, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schema, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> List(OraObject tableOrView, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return List(conn, tableOrView, filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> List(OracleConnection conn, OraObject tableOrView, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var where = Filter.And
                (
                    Filter.Equals("TABLE_NAME", tableOrView.Name)
                );
                if (tableOrView.Schema != null)
                {
                    where.Add(Filter.Equals("OWNER", tableOrView.Schema.Name));
                }

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
	                    " + where.Render(DbPlatform.Oracle) + @"
                    ORDER BY 
	                    COLUMN_ID
                ";

                var columns = Query.SQL.AsCollection
                (
                    conn,
                    sql,
                    rowParser: RowParser.From((reader) => new OraColumn
                    (
                        name: reader["name"] as string,
                        columnType: OracleTypes.LookupColumnType(reader["type"] as string)
                    )
                    {
                        Parent = tableOrView.Schema != null ? tableOrView : tableOrView.SetSchemaByName(reader["schema"] as string),
                        Length = Zap.To<int>(reader["length"]),
                        Precision = Zap.To<int?>(reader["precision"]),
                        Scale = Zap.To<int?>(reader["scale"]),
                        IsNullable = Zap.To<bool>(reader["isNullable"]),
                        ColumnID = Zap.To<int>(reader["columnID"]),
                        DefaultValue = Zap.To<string>(reader["defaultValue"])
                    }),
                    commandTimeout: commandTimeout
                );

                if (filter != null)
                {
                    return columns.Where(c => filter.Invoke(c)).ToList();
                }
                return columns;
            }

            public static OraColumn Lookup(string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(out OracleConnection conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);      /* uses default dataSource, credentials */
                using (conn)
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraColumn Lookup(OraServer dataSource, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = dataSource ?? throw new UtilityException("Server cannot be null"), Credentials = credentials }))
                {
                    return Lookup(conn, dataSource, schemaName, tableOrViewName, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraServer dataSource, string schemaName, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, dataSource, schemaName, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraColumn Lookup(OraSchema schema, string tableOrViewName, string name, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = schema.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Schema"), Credentials = credentials }))
                {
                    return Lookup(conn, schema, tableOrViewName, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraSchema schema, string tableOrViewName, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var tableOrView = Objects.Lookup(conn, schema, tableOrViewName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
            }

            public static OraColumn Lookup(OraObject tableOrView, string name, Credential? credentials = null, bool ignoreCase = false, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return Lookup(conn, tableOrView, name, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static OraColumn Lookup(OracleConnection conn, OraObject tableOrView, string name, bool ignoreCase = false, int? commandTimeout = null)
            {
                var filteredList = List
                (
                    conn,
                    tableOrView,
                    filter: (c) => c.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                    commandTimeout: commandTimeout
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

            public static IEnumerable<OraColumn> SearchByName(OraObject tableOrView, IComparator<string> comparator, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, tableOrView, comparator, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByName(OracleConnection conn, OraObject tableOrView, IComparator<string> comparator, int? commandTimeout = null)
            {
                var columns = List(conn, tableOrView, commandTimeout: commandTimeout);
                columns = columns
                    .Where(col => comparator.IsMatch(col.Name))
                    .ToList();
                return columns;
            }

            public static IEnumerable<OraColumn> SearchByName(OraObject tableOrView, string partialName, bool ignoreCase = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByName(conn, tableOrView, partialName, ignoreCase: ignoreCase, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByName(OracleConnection conn, OraObject tableOrView, string partialName, bool ignoreCase = false, int? commandTimeout = null)
            {

                var comparator = new Comparator<string>
                {
                    Mode = CompareMode.Contains,
                    Criteria = ObjectValues.From(partialName),
                    IgnoreCase = ignoreCase
                };
                return SearchByName(conn, tableOrView, comparator, commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> SearchByValue(OraObject tableOrView, object value, Predicate<OraColumn>? filter = null, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchByValue(conn, tableOrView, value, filter: filter, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> SearchByValue(OracleConnection conn, OraObject tableOrView, object value, Predicate<OraColumn>? filter = null, int? commandTimeout = null)
            {
                var columns = List(conn, tableOrView, filter: filter, commandTimeout: commandTimeout);
                IEnumerable<object?> dictinctValues;
                var matchingColumns = new List<OraColumn>();
                foreach (var column in columns)
                {
                    dictinctValues = GetDistinctValues(conn, column, commandTimeout);
                    if (value is Comparator<string> stringComparitor)
                    {
                        foreach (object? dictinctValue in dictinctValues)
                        {
                            if (stringComparitor.IsMatch(dictinctValue?.ToString()))
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

            public static IEnumerable<OraColumn> SearchTextColumnsByValue(OraObject tableOrView, string value, bool ignoreCase = false, int? commandTimeout = null)
            {
                return SearchTextColumnsByCriteria(tableOrView, Comparator.Equals(value, ignoreCase: ignoreCase), commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByValue(OracleConnection conn, OraObject tableOrView, string value, bool ignoreCase = false, int? commandTimeout = null)
            {
                return SearchTextColumnsByCriteria(conn, tableOrView, Comparator.Equals(value, ignoreCase: ignoreCase), commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByCriteria(OraObject tableOrView, IComparator<string> comparator, int? commandTimeout = null)
            {
                return SearchByValue(tableOrView, comparator, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByCriteria(OracleConnection conn, OraObject tableOrView, IComparator<string> comparator, int? commandTimeout = null)
            {
                return SearchByValue(conn, tableOrView, comparator, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
            }

            private static IEnumerable<object> GetDistinctValues(OracleConnection conn, OraColumn column, int? commandTimeout)
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
                    rowParser: new RowParser<object>( (object[] objects) => objects[0]),
                    commandTimeout: commandTimeout,
                    autoTrunc: AutoTruncate.Zap
                );
                return distinctValues;
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByHasNonprintableCharacters(OraObject tableOrView, bool distinct = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = tableOrView.Server ?? throw new UtilityException("No Server ancestor exists for the supplied " + (tableOrView?.ObjectType.ToString() ?? "Object")), Credentials = credentials }))
                {
                    return SearchTextColumnsByHasNonprintableCharacters(conn, tableOrView, distinct: distinct, commandTimeout: commandTimeout);
                }
            }

            public static IEnumerable<OraColumn> SearchTextColumnsByHasNonprintableCharacters(OracleConnection conn, OraObject tableOrView, bool distinct = false, int? commandTimeout = null)
            {
                var columns = List(conn, tableOrView, filter: (column) => column.IsText(), commandTimeout: commandTimeout);
                IEnumerable<string> nonPrintValues;
                var matchingColumns = new List<OraColumn>();
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

            public static IEnumerable<string> GetValuesWithNonprintableCharacters(OraColumn column, bool distinct = false, Credential? credentials = null, int? commandTimeout = null)
            {
                using (var conn = OracleDbUtil.LaunchConnection(connectionInfo: new OracleDbConnectionInfo { DataSource = column.Server ?? throw new UtilityException("No Server ancestor exists for the supplied Column"), Credentials = credentials }))
                {
                    conn.Open();
                    return GetValuesWithNonprintableCharacters(conn, column, distinct: distinct, commandTimeout: commandTimeout);
                }
            }

            private static IEnumerable<string> GetValuesWithNonprintableCharacters(OracleConnection conn, OraColumn column, bool distinct, int? commandTimeout)
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
                    rowParser: RowParser.ScalarString,
                    commandTimeout: commandTimeout
                );
                return nonPrintValues;
            }
        }
    }
}
