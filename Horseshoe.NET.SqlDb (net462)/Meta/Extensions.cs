using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.SqlDb.Meta
{
    public static class Extensions
    {
        public static IEnumerable<Db> ListDatabases(this DbServer server, Predicate<Db> filter = null, int? timeout = null)
        {
            return MetaSearch.Databases.List(server, filter: filter, timeout: timeout);
        }

        public static IEnumerable<DbObject> ListObjects(this Db database, SqlObjectType? objectType = null, Predicate<DbObject> filter = null, int? timeout = null)
        {
            return MetaSearch.Objects.List(database, objectType: objectType, filter: filter, timeout: timeout);
        }

        public static IEnumerable<DbColumn> ListColumns(this DbObject dbObject, Predicate<DbColumn> filter = null, int? timeout = null)
        {
            return MetaSearch.Columns.List(dbObject, filter: filter, timeout: timeout);
        }

        public static DbColumn LookupColumn(this DbObject dbObject, string columnName, bool ignoreCase = false, int? timeout = null)
        {
            return MetaSearch.Columns.Lookup(dbObject, columnName, ignoreCase: ignoreCase, timeout: timeout);
        }

        public static bool IsText(this DbColumn column)
        {
            return SqlServerTypes.IsText(column.ColumnType);
        }

        public static bool IsNumeric(this DbColumn column)
        {
            return SqlServerTypes.IsNumeric(column.ColumnType);
        }

        public static bool IsDate(this DbColumn column)
        {
            return SqlServerTypes.IsDate(column.ColumnType);
        }
    }
}
