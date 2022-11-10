using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb.Meta
{
    public static class Extensions
    {
        public static IEnumerable<OraSchema> ListSchemas(this OraServer server, Credential? credentials = null, Predicate<OraSchema> filter = null, int? timeout = null)
        {
            return MetaSearch.Schemas.List(server, credentials: credentials, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraSchema> ListSchemas(this OraServer server, OracleConnection conn, Predicate<OraSchema> filter = null, int? timeout = null)
        {
            return MetaSearch.Schemas.List(conn, server, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraServer server, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(server, credentials: credentials, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraServer server, OracleConnection conn, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(conn, server, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraServer server, string schemaName, Credential? credentials = null, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(server, schemaName, credentials: credentials, ignoreCase: ignoreCase, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraServer server, OracleConnection conn, string schemaName, bool ignoreCase = false, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(conn, server, schemaName, ignoreCase: ignoreCase, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraSchema schema, Credential? credentials = null, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(schema, credentials: credentials, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraObject> ListObjects(this OraSchema schema, OracleConnection conn, OraObjectType? objectType = null, Predicate<OraObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(conn, schema, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<OraColumn> ListColumns(this OraObject dbObject, Predicate<OraColumn> filter = null, int? timeout = null)
        {
            return MetaSearch.Columns.List(dbObject, filter: filter, timeout: timeout);
        }

        public static OraSchema LookupSchema(this OraServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Schemas.Lookup(server, name, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraSchema LookupSchema(this OraServer server, OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Schemas.Lookup(conn, server, name, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraServer server, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(server, name, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraServer server, OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(conn, server, name, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraServer server, string schemaName, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(server, name, schemaName, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraServer server, OracleConnection conn, string schemaName, string name, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(conn, server, schemaName, name, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraSchema schema, string name, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(schema, name, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraObject LookupObject(this OraSchema schema, OracleConnection conn, string name, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Objects.Lookup(conn, schema, name, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraColumn LookupColumn(this OraObject dbObject, string columnName, Credential? credentials = null, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Columns.Lookup(dbObject, columnName, credentials: credentials, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static OraColumn LookupColumn(this OraObject dbObject, OracleConnection conn, string columnName, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Columns.Lookup(conn, dbObject, columnName, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static bool IsText(this OraColumn column)
        {
            return OracleTypes.IsText(column.ColumnType);
        }

        public static bool IsNumeric(this OraColumn column)
        {
            return OracleTypes.IsNumeric(column.ColumnType);
        }

        public static bool IsDate(this OraColumn column)
        {
            return OracleTypes.IsDate(column.ColumnType);
        }
    }
}
